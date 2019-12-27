using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Auth;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.Shared.Auth;
using BitContainer.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BitContainer.AuthService.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersProvider _usersProvider;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersProvider usersProvider, ILogger<UsersController> logger)
        {
            _usersProvider = usersProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("user/{name}")]
        public ActionResult<CUserContract> GetUser(String name)
        {
            CUser user = _usersProvider.GetUserWithName(name);
            if (user == null) return BadRequest("No such user");
            return new CUserContract(user.Id, user.Name);
        }

        [HttpPost]
        [Route("register")]
        public ActionResult Register(CCredentialsContract credentials)
        {
            CUser user = _usersProvider.GetUserWithName(credentials.UserName);
            if (user != null) return Conflict($"User {user.Name} already exists");

            Byte[] salt = CryptoHelper.GenerateSalt();
            Byte[] hashedHash = CryptoHelper.GenerateHash(credentials.Hash, salt);

            _usersProvider.AddNewUser(credentials.UserName, hashedHash, salt);

            _logger.LogInformation($"User '{credentials.UserName}' was registered.");

            return Ok();
        }
        
        [HttpPost]
        [Route("logIn")]
        public async Task LogIn(CCredentialsContract credentials)
        {
            Byte[] salt = _usersProvider.GetSalt(credentials.UserName);
            if (salt == null)
            {
                Response.ContentType = "application/json";
                Response.StatusCode = (Int32) HttpStatusCode.BadRequest;
                await Response.WriteAsync((new ErrorDetails()
                {
                    StatusCode = Response.StatusCode,
                    Message = "No such user."
                }).ToString());
                return;
            }
            
            Byte[] hashedHash = CryptoHelper.GenerateHash(credentials.Hash, salt);
            
            CUser user = _usersProvider.GetUserWithCredentials(credentials.UserName, hashedHash);

            if (user == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username or password.");
                return;
            }

            ClaimsIdentity identity = GetIdentity(user);

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(AuthOptions.LifeTime),
                signingCredentials: 
                    new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var tokenHandler = new JwtSecurityTokenHandler();
            var encodedJwt = tokenHandler.WriteToken(jwt);

            CUserStats stats = _usersProvider.GetStats(user.Id);

            CAuthenticatedUserContract response = 
                new CAuthenticatedUserContract(
                    encodedJwt, 
                    user.Id, 
                    user.Name, 
                    stats.FilesCount, 
                    stats.DirsCount, 
                    stats.StorageSize);

            Response.ContentType = "applications/json";

            _logger.LogInformation($"User '{user.Name}' logged in.");

            await Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private ClaimsIdentity GetIdentity(CUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
            return claimsIdentity;
        }

    }
}
