using System;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.ViewModels.Controls;

namespace BitContainer.Presentation.ViewModels
{
    public class MainPageViewModel : WaitingViewModelBase, IDisposable
    {
        private readonly FileSystemController _fileSystemController;

        private MenuControlViewModel _menu;

        public MenuControlViewModel Menu
        {
            get => _menu;
            set
            {
                _menu = value;
                OnPropertyChanged();
            }
        }

        private DirTreeControlViewModel _dirTree;
        public DirTreeControlViewModel DirTree
        {
            get => _dirTree;
            set
            {
                _dirTree = value;
                OnPropertyChanged();
            }
        }
        private ExplorerControlViewModel _explorer;
        public ExplorerControlViewModel Explorer
        {
            get => _explorer;
            set
            {
                _explorer = value;
                OnPropertyChanged();
            }
        }
        private LoadsPanelViewModel _loadsPanel;
        public LoadsPanelViewModel LoadsPanel
        {
            get => _loadsPanel;
            set
            {
                _loadsPanel = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel(FileSystemController fileSystemController)
        {
            _fileSystemController = fileSystemController;

            Menu = new MenuControlViewModel();
            DirTree = new DirTreeControlViewModel(_fileSystemController);
            Explorer = new ExplorerControlViewModel(_fileSystemController);
            LoadsPanel = new LoadsPanelViewModel();

            Init();
        }

        private void Init()
        {
            Explorer.SelectEntity(_fileSystemController.Root);
        }

        public void Dispose()
        {
            DirTree.Dispose();
            Explorer.Dispose();
            LoadsPanel.Dispose();
        }
    }
}
