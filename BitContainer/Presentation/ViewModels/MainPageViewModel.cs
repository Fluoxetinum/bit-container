using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.ViewModels.Jobs;

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

        public MainPageViewModel()
        {
            _fileSystemController = new FileSystemController();

            Menu = new MenuControlViewModel();
            DirTree = new DirTreeControlViewModel(_fileSystemController);
            Explorer = new ExplorerControlViewModel(_fileSystemController);
            LoadsPanel = new LoadsPanelViewModel();

            Init();
        }

        private async void Init()
        {
            await Explorer.LoadDirectory(_fileSystemController.Root);
        }

        public void Dispose()
        {
            DirTree.Dispose();
            Explorer.Dispose();
            LoadsPanel.Dispose();
        }
    }
}
