using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Presentation.Models;
using MaterialDesignThemes.Wpf;

namespace BitContainer.Presentation.ViewModels.Jobs
{
    class UploadJob : AbstractJob
    {
        public static UploadJob Create(String name, Double progress = 0)
        {
            return new UploadJob()
            {
                Icon = PackIconKind.Upload,
                Name = name,
                Progress = progress
            };
        }

    }
}
