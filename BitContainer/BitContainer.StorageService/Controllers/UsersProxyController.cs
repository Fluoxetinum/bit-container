using System;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Auth;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.Shared.Http;
using BitContainer.Shared.Http.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BitContainer.StorageService.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersProxyController : ControllerBase
    {
        private readonly IStatsProvider _statsProvider;

        public UsersProxyController(IStatsProvider statsProvider)
        {
            _statsProvider = statsProvider;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(CCredentialsContract credentials)
        {
            ActionResult requestResult = Ok();

            try
            {
                await AuthServiceProxy.RegisterRequest(credentials);
                CUserContract user = await AuthServiceProxy.GetUserWithName(credentials.UserName);
                _statsProvider.AddNewStats(user.Id);
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
                CUserStats stats = _statsProvider.GetStats(contract.User.Id);
                contract.Stats = new CStatsContract(stats.FilesCount, stats.DirsCount, stats.StorageSize);
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
