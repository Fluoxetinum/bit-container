using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Dialogs;
using BitContainer.Presentation.ViewModels.Enums;
using BitContainer.Presentation.ViewModels.Nodes;
using BitContainer.Presentation.Views.Dialogs;
using BitContainer.Shared.Models;
using Microsoft.Win32;

namespace BitContainer.Presentation.ViewModels.Controls
{
    public class ExplorerControlViewModel : NavigatableViewModelBase, IDisposable
    {
        private readonly FileSystemController _fileSystemController;
        private readonly ArrangeController _arrangeController;

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

        private FileSystemNode CurrentNode => Path.CurrentPath.Last();
        
        private Dictionary<String, ObservableCollection<FileSystemNode>> _storageEntities;
        public Dictionary<String, ObservableCollection<FileSystemNode>> StorageEntities
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

        public ExplorerControlViewModel(FileSystemController fileSystemController)
        {
            _fileSystemController = fileSystemController;
            Path = new PathControlViewModel(fileSystemController);

            _arrangeController = new ArrangeController();
            StorageEntities = _arrangeController.Arrangement;

            SelectedGroupKey = GroupType.None; 
            SelectedSortKey = SortType.Name;

            _fileSystemController.FileSystemEvents.DirectoryOpened += 
                OnDirectoryOpened;
            _fileSystemController.FileSystemEvents.StorageEntityCreated += 
                OnStorageEntityCreated;
            _fileSystemController.FileSystemEvents.StorageEntityDeleted += 
                OnStorageEntityDeleted;
        }

        public void Dispose()
        {
            _fileSystemController.FileSystemEvents.DirectoryOpened -= 
                OnDirectoryOpened;
            _fileSystemController.FileSystemEvents.StorageEntityCreated -= 
                OnStorageEntityCreated;
            _fileSystemController.FileSystemEvents.StorageEntityDeleted -= 
                OnStorageEntityDeleted;
        }

        #region EventCallbacks

        public void OnStorageEntityDeleted(object sender, NodeChangedEventArgs e)
        {
            _arrangeController.RemoveFromArrangement(e.Node);
        }

        public void OnStorageEntityCreated(object sender, NodeChangedEventArgs e)
        {
            _arrangeController.AddToArrangement(e.Node);
        }

        public async void OnDirectoryOpened(object sender, NodeOpenedEventArgs e)
        {
            StorageEntities = await _arrangeController.Reset(e.Children, SelectedGroupKey, SelectedSortKey);
        }

        #endregion

        private ICommand _selectEntityCommand;
        public ICommand SelectEntityCommand =>
            _selectEntityCommand ??= new RelayCommand<FileSystemNode>(SelectEntity);

        public async void SelectEntity(FileSystemNode selectedNode)
        {
            if (selectedNode.IsDir) await _fileSystemController.OpenDirectory(selectedNode);
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new RelayCommand(Search);

        public async void Search(Object data)
        {
            await _fileSystemController.Search(CurrentNode, SearchString);
        }

        private ICommand _downloadEntityCommand;

        public ICommand DownloadEntityCommand =>
            _downloadEntityCommand ??= new RelayCommand<FileSystemNode>(DownloadEntity);

        public async void DownloadEntity(FileSystemNode selectedNode)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                CheckPathExists = true,
                FileName = selectedNode.Name,
            };

            if (dialog.ShowDialog() != true) return;

            await _fileSystemController.DownloadEntity(dialog.FileName, selectedNode);
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

            await _fileSystemController.CreateDirectory(dialog.InputField, CurrentNode);
        }

        private ICommand _uploadFileCommand;

        public ICommand UploadFileCommand =>
            _uploadFileCommand ??= new RelayCommand(UploadFile);

        public async void UploadFile(object data)
        {
            var filePicker = new OpenFileDialog { CheckFileExists = true };

            if (filePicker.ShowDialog() != true) return;

            await _fileSystemController.UploadFile(filePicker.FileName, CurrentNode);
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

        public async void GroupBy(GroupType groupType)
        {
            StorageEntities = await _arrangeController.Arrange(groupType);
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

        public async void SortBy(SortType sortType)
        {
            StorageEntities = await _arrangeController.Arrange(sortType);
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand =>
            _deleteCommand ??= new RelayCommand<FileSystemNode>(Delete);

        public async void Delete(FileSystemNode node)
        {
            await _fileSystemController.Delete(node);
        }

        private ICommand _renameCommand;
        public ICommand RenameCommand =>
            _renameCommand ??= new RelayCommand<FileSystemNode>(Rename);

        public async void Rename(FileSystemNode node)
        {
            StringInputDialog dialog = new StringInputDialog("Enter new name", node.Name)
            {
                Title = "Renaming",
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (dialog.ShowDialog() != true) return;
            if (dialog.InputField.Equals(node.Name)) return;

            await _fileSystemController.Rename(node, dialog.InputField);
        }
        
        private ICommand _shareCommand;

        public ICommand ShareCommand =>
            _shareCommand ??= new RelayCommand<FileSystemNode>(Share);

        public async void Share(FileSystemNode node)
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
            EAccessType access = dialog.Access;

            await _fileSystemController.UpdateShare(user, access, node);
        }

        private ICommand _copyCommand;

        public ICommand CopyCommand =>
            _copyCommand ??= new RelayCommand<FileSystemNode>(Copy);

        public async void Copy(FileSystemNode node)
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

            if (dialog.ShowDialog() == true)
            {
                FileSystemNode parent = vm.SelectedParent;
                await _fileSystemController.Copy(node, parent);
            }

            vm.Dispose();
        }
    }
}
