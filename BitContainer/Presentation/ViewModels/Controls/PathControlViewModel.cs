using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.ViewModels.Controls
{
    public class PathControlViewModel : ViewModelBase, IDisposable
    {
        private readonly FileSystemController _fileSystemController;

        private ObservableCollection<CFileSystemNode> _currentPath;
        public ObservableCollection<CFileSystemNode> CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        public PathControlViewModel(FileSystemController fileSystemController)
        {
            _fileSystemController = fileSystemController;

            CurrentPath = new ObservableCollection<CFileSystemNode>()
            {
                _fileSystemController.Root
            };

            _fileSystemController.FileSystemEvents.DirectoryOpened += EventsControllerOnDirectoryOpened;
        }

        public async void EventsControllerOnDirectoryOpened(object sender, FsNodeEventArgs e)
        {
            LinkedList<CFileSystemNode> path = await _fileSystemController.ComputePath(e.Node);
            CurrentPath = new ObservableCollection<CFileSystemNode>(path);
        }

        private ICommand _selectEntityCommand;
        public ICommand SelectEntityCommand =>
            _selectEntityCommand ??= new RelayCommand<CFileSystemNode>(SelectEntity);

        public async void SelectEntity(CFileSystemNode dir)
        {
            await _fileSystemController.FetchChildren(dir);
        }
        
        public void Dispose()
        {
            _fileSystemController.FileSystemEvents.DirectoryOpened -= EventsControllerOnDirectoryOpened;
        }
    }
}
