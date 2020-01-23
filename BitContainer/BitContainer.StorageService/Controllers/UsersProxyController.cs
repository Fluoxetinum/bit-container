using System.Threading.Tasks;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Http.Exceptions;
using BitContainer.Http.Proxies;
using BitContainer.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BitContainer.Service.Storage.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersProxyController : ControllerBase
    {
        private readonly IStatsProvider _statsProvider;
        private readonly ISqlDbHelper _sqlDbHelper;
        private readonly IAuthServiceProxy _authServiceProxy;

        public UsersProxyController(IStatsProvider statsProvider, IAuthServiceProxy authServiceProxy, ISqlDbHelper dbHelper)
        {
            _statsProvider = statsProvider;
            _authServiceProxy = authServiceProxy;
            _sqlDbHelper = dbHelper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(CCredentialsContract credentials)
        {
            ActionResult requestResult = Ok();

            try
            {
                await _sqlDbHelper.ExecuteTransactionAsync(async (command) =>
                {
                    await _authServiceProxy.RegisterRequest(credentials);
                    CUserContract user = await _authServiceProxy.GetUserWithName(credentials.UserName);
                    _statsProvider.AddNewStats(command, new CUserId(user.Id));
                });
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
                contract = await _authServiceProxy.LogInRequest(credentials);
                CStats stats = _statsProvider.GetStats(new CUserId(contract.User.Id));
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
