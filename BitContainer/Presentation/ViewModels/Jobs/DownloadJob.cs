using System;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;
using MaterialDesignThemes.Wpf;

namespace BitContainer.Presentation.ViewModels.Jobs
{
    public class DownloadJob : AbstractJob
    {
        public static DownloadJob Create(String name, Double progress = 0)
        {
            return new DownloadJob()
            {
                Icon = PackIconKind.Download,
                Name = name,
                Progress = progress
            };
        }
    }
}
