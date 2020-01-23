using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.ViewModels.Controls
{
    public class PathControlViewModel : ViewModelBase, IDisposable
    {
        private readonly FileSystemController _fileSystemController;

        private ObservableCollection<FileSystemNode> _currentPath;
        public ObservableCollection<FileSystemNode> CurrentPath
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

            CurrentPath = new ObservableCollection<FileSystemNode>()
            {
                _fileSystemController.Root
            };

            _fileSystemController.FileSystemEvents.DirectoryOpened += EventsControllerOnDirectoryOpened;
        }

        public async void EventsControllerOnDirectoryOpened(object sender, NodeOpenedEventArgs e)
        {
            LinkedList<FileSystemNode> path = await _fileSystemController.GetPathFromRoot(e.Node);
            CurrentPath = new ObservableCollection<FileSystemNode>(path);
        }

        private ICommand _selectEntityCommand;
        public ICommand SelectEntityCommand =>
            _selectEntityCommand ??= new RelayCommand<FileSystemNode>(SelectEntity);

        public async void SelectEntity(FileSystemNode dir)
        {
            await _fileSystemController.OpenDirectory(dir);
        }
        
        public void Dispose()
        {
            _fileSystemController.FileSystemEvents.DirectoryOpened -= EventsControllerOnDirectoryOpened;
        }
    }
}
