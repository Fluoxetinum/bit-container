using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using BitContainer.Http;
using BitContainer.Http.Proxies;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Service;

namespace BitContainer.Presentation.Controllers.Ui
{
    public class DependecyController
    {
        private static readonly String MainServiceUrl;
        private static readonly CHttpHelper HttpHelper;

        static DependecyController()
        {
            MainServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
            HttpHelper = new CHttpHelper();
        }

        public static FileSystemController GetFileSystemController()
        {
            var storageServiceProxy = new CStorageServiceProxy(HttpHelper, MainServiceUrl);
            var storageController = new CStorageController(storageServiceProxy);
            var eventsController = new FileSystemEventsController(); 
            return new FileSystemController(storageController, eventsController);
        }

        public static CAuthController GetAuthController()
        {
            var authServiceProxy = new СAuthServiceProxy(HttpHelper, MainServiceUrl);
            return new CAuthController(authServiceProxy);
        }

    }
}
