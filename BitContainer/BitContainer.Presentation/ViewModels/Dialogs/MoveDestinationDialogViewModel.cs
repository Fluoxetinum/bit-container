﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Controls;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.ViewModels.Dialogs
{
    public class MoveDestinationDialogViewModel : WaitingViewModelBase, IDisposable
    {
        private readonly FileSystemController _fileSystemController;

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

        public FileSystemNode SelectedParent =>
            Path.CurrentPath.Last();
        
        public MoveDestinationDialogViewModel()
        {
            _fileSystemController = DependecyController.GetFileSystemController();
            DirTree = new DirTreeControlViewModel(_fileSystemController);
            Path = new PathControlViewModel(_fileSystemController);
        }

        public void Dispose()
        {
            DirTree.Dispose();
            Path.Dispose();
        }
    }
}
