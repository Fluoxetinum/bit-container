using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.Icons;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Controls;
using BitContainer.Presentation.ViewModels.Dialogs;
using BitContainer.Presentation.ViewModels.Enums;
using BitContainer.Presentation.Views;
using BitContainer.Presentation.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace BitContainer.Presentation.ViewModels
{
    public class ExplorerControlViewModel : NavigatableViewModelBase, IDisposable
    {
        private FileSystemController _fileSystemController;

        private PathControlViewModel _path;
        public PathControlViewModel Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        
        private Dictionary<String, ObservableCollection<CFileSystemNode>> _storageEntities;
        public Dictionary<String, ObservableCollection<CFileSystemNode>> StorageEntities
        {
            get => _storageEntities;
            set
            {
                _storageEntities = value;
                OnPropertyChanged();
            }
        }

        private String _searchString;
        public String SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged();
            }
        }

        private ICommand _infoCommand;
        public ICommand InfoCommand => 
            _infoCommand ??= 
                new RelayCommand<CFileSystemNode>(ShowInfo);

        public void ShowInfo(CFileSystemNode node)
        {

        }


        private ICommand _searchCommand;
        public ICommand SearchCommand => 
            _searchCommand ??= new RelayCommand(Search);

        public async void Search(Object data)
        {
            String pattern = SearchString;
            CFileSystemNode node = Path.CurrentPath.Last();

            List<IAccessWrapperUiModel> result = new List<IAccessWrapperUiModel>();

            if (node.AccessWrapper is COwnStorageEntityUiModel)
                result = await StorageController.SearchOwnerEntities(node.Entity.Id, pattern);
            else if (node.AccessWrapper is CRestrictedStorageEntityUiModel)
                result = await StorageController.SearchSharedEntities(node.Entity.Id, pattern);
            
            FillPanelWithSearchResults(result);
        }

        public ExplorerControlViewModel(FileSystemController fileSystemController)
        {
            _fileSystemController = fileSystemController;
            StorageEntities = new Dictionary<String, ObservableCollection<CFileSystemNode>>();
            Path = new PathControlViewModel(fileSystemController);

            SelectedGroupKey = GroupType.None; 
            SelectedSortKey = SortType.Name;

            _fileSystemController.FileSystemEvents.DirectoryOpened += 
                EventsControllerOnDirectoryOpened;
            _fileSystemController.FileSystemEvents.StorageEntityCreated += 
                EventsControllerOnStorageEntityCreated;
            _fileSystemController.FileSystemEvents.StorageEntityDeleted += 
                EventsControllerOnStorageEntityDeleted;
        }

        public void EventsControllerOnStorageEntityDeleted(object sender, FsNodeEventArgs e)
        {
            switch (SelectedGroupKey)
            {
                case GroupType.Created: 
                    StorageEntities[e.Node.CreatedDate.ToLongDateString()].Remove(e.Node);
                    break;
                case GroupType.None:
                    StorageEntities[String.Empty].Remove(e.Node); 
                    break;
                default:
                    throw new NotSupportedException("Not supported grouping type.");
            }
        }

        public void EventsControllerOnStorageEntityCreated(object sender, FsNodeEventArgs e)
        {
            switch (SelectedGroupKey)
            {
                case GroupType.Created: 
                    StorageEntities[e.Node.CreatedDate.ToLongDateString()].Add(e.Node);
                    break;
                case GroupType.None:
                    StorageEntities[String.Empty].Add(e.Node); 
                    break;
                default:
                    throw new NotSupportedException("Not supported grouping type.");
            }
        }

        public void EventsControllerOnDirectoryOpened(object sender, FsNodeEventArgs e)
        {
            var dict = new Dictionary<String, ObservableCollection<CFileSystemNode>>
            {
                [String.Empty] = e.Node.Children
            };
            StorageEntities = dict;
        }

        public void FillPanelWithSearchResults(List<IAccessWrapperUiModel> results)
        {
            ObservableCollection<CFileSystemNode> searchResutlToFill = 
                new ObservableCollection<CFileSystemNode>();

            foreach (var result in results)
            {
                CFileSystemNode node = new CFileSystemNode(null, result);
                searchResutlToFill.Add(node);
            }

            var dict = new Dictionary<String, ObservableCollection<CFileSystemNode>>
            {
                [String.Empty] = searchResutlToFill
            };

            StorageEntities = dict;
        }

        public async Task LoadDirectory(CFileSystemNode dir)
        {
            LinkedList<Guid> downList = dir.GetDownList();

            if (downList == null)
            {
                await _fileSystemController.FetchChildren(dir);
            }
            else
            {

                Boolean shared = false;

                if (dir.AccessWrapper is CSearchResultUiModelDirtyAdapter adapter)
                {
                    switch (adapter.AccessWrapper)
                    {
                        case CRestrictedStorageEntityUiModel restricted:
                            shared = true;
                            break;
                        case COwnStorageEntityUiModel own:
                            shared = false;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                _fileSystemController.FileSystemEvents.DirectoryOpened -= 
                    EventsControllerOnDirectoryOpened;

                var node = downList.First;

                while (node != downList.Last)
                {
                    await _fileSystemController.FetchChildren(node.Value, shared);
                    node = node.Next;
                }

                _fileSystemController.FileSystemEvents.DirectoryOpened += 
                    EventsControllerOnDirectoryOpened;

                await _fileSystemController.FetchChildren(downList.Last.Value, shared);
                
            }
        }
        
        private ICommand _selectEntityCommand;

        public ICommand SelectEntityCommand =>
            _selectEntityCommand ??= new RelayCommand<CFileSystemNode>(SelectEntity);

        public async void SelectEntity(CFileSystemNode selectedNode)
        {
            if (selectedNode.IsFile)
            {
                // No action by now (Andrey Gurin)
            }
            else if (selectedNode.IsDir)
            {
                await LoadDirectory(selectedNode);
            }
        }

        private ICommand _downloadEntityCommand;

        public ICommand DownloadEntityCommand =>
            _downloadEntityCommand ??= new RelayCommand<CFileSystemNode>(DownloadEntity);

        public async void DownloadEntity(CFileSystemNode selectedNode)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                CheckPathExists = true,
                FileName = selectedNode.Entity.Name,
            };

            if (dialog.ShowDialog() != true) return;

            await StorageController.LoadEntity(dialog.FileName, selectedNode.Entity);
        }

        private ICommand _createDirCommand;
        public ICommand CreateDirCommand =>
            _createDirCommand ??= new RelayCommand(CreateDir);

        public async void CreateDir(object data)
        {
            StringInputDialog dialog = new StringInputDialog("Enter directory's name")
            {
                Title = "Create new directory",
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (dialog.ShowDialog() != true) return;

            await _fileSystemController.CreateDirectory(dialog.InputField,Path.CurrentPath.Last());
        }

        private ICommand _uploadFileCommand;

        public ICommand UploadFileCommand =>
            _uploadFileCommand ??= new RelayCommand(UploadFile);

        public async void UploadFile(object data)
        {
            var filePicker = new OpenFileDialog { CheckFileExists = true };

            if (filePicker.ShowDialog() != true) return;

            await _fileSystemController.UploadFile(filePicker.FileName, Path.CurrentPath.Last());
        }

        private GroupType _selectedGroupKey;

        public GroupType SelectedGroupKey
        {
            get => _selectedGroupKey;
            set
            {
                _selectedGroupKey = value;
                GroupByCommand.Execute(_selectedGroupKey);
                OnPropertyChanged();
            }
        }

        private ICommand _groupByCommand;

        public ICommand GroupByCommand =>
            _groupByCommand ??= new RelayCommand<GroupType>(GroupBy);

        public async void GroupBy(GroupType type)
        {
            switch (type)
            {
                case GroupType.Created: 
                    StorageEntities = await ArrangeController.GroupByCreationDate(StorageEntities);
                    break;
                case GroupType.None:
                    StorageEntities = await ArrangeController.DisableGroupping(StorageEntities);
                    break;
                default:
                    throw new NotSupportedException("Not supported grouping type.");
            }
        }

        private SortType _selectedSortKey;

        public SortType SelectedSortKey
        {
            get => _selectedSortKey;
            set
            {
                _selectedSortKey = value;
                SortByCommand.Execute(_selectedSortKey);
                OnPropertyChanged();
            }
        }
        
        private ICommand _sortByCommand;

        public ICommand SortByCommand =>
            _sortByCommand ??= new RelayCommand<SortType>(SortBy);

        public async void SortBy(SortType type)
        {
            switch (type)
            {
                case SortType.Created: 
                    StorageEntities = await ArrangeController.SortByCreationTime(StorageEntities);
                    break;
                case SortType.Name:
                    StorageEntities = await ArrangeController.SortByName(StorageEntities);
                    break;
                default:
                    throw new NotSupportedException("Not supported sorting type.");
            }
        }

        private ICommand _deleteCommand;

        public ICommand DeleteCommand =>
            _deleteCommand ??= new RelayCommand<CFileSystemNode>(Delete);

        public async void Delete(CFileSystemNode node)
        {
            await _fileSystemController.Delete(node);
        }

        private ICommand _renameCommand;

        public ICommand RenameCommand =>
            _renameCommand ??= new RelayCommand<CFileSystemNode>(Rename);

        public async void Rename(CFileSystemNode node)
        {
            StringInputDialog dialog = new StringInputDialog("Enter new name", node.Entity.Name)
            {
                Title = "Renaming",
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (dialog.ShowDialog() != true) return;

            if (dialog.InputField.Equals(node.Entity.Name)) return;

            await _fileSystemController.Rename(node, dialog.InputField);
        }

        private ICommand _editTextFileCommand;

        public ICommand EditTextFileCommand =>
            _editTextFileCommand ??= new RelayCommand<CFileSystemNode>(Edit);

        public void Edit(CFileSystemNode node)
        {
            ToEditPage();
        }

        private ICommand _shareCommand;

        public ICommand ShareCommand =>
            _shareCommand ??= new RelayCommand<CFileSystemNode>(Share);

        public async void Share(CFileSystemNode node)
        {
            ShareDialog dialog  = new ShareDialog()
            {
                Title = "Sharing",
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dialog.ShowDialog() != true) return;

            String user = dialog.UserName;
            EAccessTypeUiModel access = dialog.Access;

            await _fileSystemController.UpdateShare(user, access, node);
        }

        private ICommand _copyCommand;

        public ICommand CopyCommand =>
            _copyCommand ??= new RelayCommand<CFileSystemNode>(Copy);

        public async void Copy(CFileSystemNode node)
        {
            var vm = new MoveDestinationDialogViewModel();
            MoveDestinationDialog dialog = new MoveDestinationDialog()
            {
                Title = "Moving",
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                DataContext = vm
            };
            //TODO: Refactor dialogs creation. (Andrey Gurin)

            if (dialog.ShowDialog() == true)
            {
                CFileSystemNode parent = vm.SelectedParent;
                await _fileSystemController.Copy(node, parent);
            }

            vm.Dispose();
        }

        public void Dispose()
        {
            _fileSystemController.FileSystemEvents.DirectoryOpened -= 
                EventsControllerOnDirectoryOpened;
            _fileSystemController.FileSystemEvents.StorageEntityCreated -= 
                EventsControllerOnStorageEntityCreated;
            _fileSystemController.FileSystemEvents.StorageEntityDeleted -= 
                EventsControllerOnStorageEntityDeleted;
        }
    }
}
