﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using BitContainer.Contracts.V1;
using BitContainer.LogService.Models;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Proxies.Exceptions;
using BitContainer.Presentation.Controllers.Proxies.Requests;
using System.Threading.Tasks;

namespace BitContainer.Shared.Http
{
    public class CLogServiceProxy 
    {
        private static String _serviceUrl = String.Empty;

        private static String _authControllerName = "logger";
        private static String _logRequest => $@"{_serviceUrl}/{_authControllerName}/log/";

        public static void SetServiceUrl(String serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public static async Task LogRequest(CNlogMessageContract message)
        {
            var requestBuilder = new PostRequestBuilder<CNlogMessageContract>(_logRequest, message);
            await HttpHelper.Request(requestBuilder, HttpHelper.EmptyCatchAction);
        }
    }
}
