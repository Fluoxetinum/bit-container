using System;
using System.Windows;
using BitContainer.Presentation.Controllers;
using NLog;

namespace BitContainer.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static String ResourcesRoot => "pack://application:,,,/BitContainer.Presentation;component/";
        public static String GetPackPath(String path) => $"{ResourcesRoot}{path}";
        
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionsHandler;
        }

        public void UnhandledExceptionsHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;

            SLogController.Logger.Warn(e,$"Client crashed with exception");

            MessageBox.Show(e.ToString());
        }

    }
}
