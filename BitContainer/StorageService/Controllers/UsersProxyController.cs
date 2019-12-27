using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Proxies.Exceptions;
using BitContainer.Presentation.Controllers.Proxies.Requests;
using BitContainer.Shared;
using BitContainer.Shared.Auth;
using BitContainer.Shared.Http;
using BitContainer.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BitContainer.StorageService.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersProxyController : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(CCredentialsContract credentials)
        {
            ActionResult requestResult = Ok();

            try
            {
                await AuthServiceProxy.RegisterRequest(credentials);
            }
            catch (UsernameExistsException e)
            {
                requestResult = Conflict(e.Message);
            }
            
            return requestResult;
        }
        
        [HttpPost]
        [Route("logIn")]
        public async Task<ActionResult<CAuthenticatedUserContract>> LogIn(CCredentialsContract credentials)
        {
            ActionResult requestResult = Ok();
            CAuthenticatedUserContract contract = null;

            try
            {
                contract = await AuthServiceProxy.LogInRequest(credentials);
            }
            catch (NoSuchUserException e)
            {
                requestResult = BadRequest(e.Message);
            }
            
            if (requestResult is BadRequestObjectResult)
                return requestResult;

            return contract;
        }
    }
}
