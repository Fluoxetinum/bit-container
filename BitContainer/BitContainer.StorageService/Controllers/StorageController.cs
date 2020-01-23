using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.ActionContracts;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Contracts.V1.Events;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Http.Exceptions;
using BitContainer.Http.Proxies;
using BitContainer.Service.Storage.Helpers;
using BitContainer.Service.Storage.Managers;
using BitContainer.Service.Storage.Managers.Interfaces;
using BitContainer.Services.Shared;
using BitContainer.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Newtonsoft.Json;

namespace BitContainer.Service.Storage.Controllers
{
    [Route("storage")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private ISignalsManager _signalsManager;

        private readonly IAuthServiceProxy _authServiceProxy;
        private readonly ILoadsManager _loadsManager;
        private readonly IStorageProvider _storage;
        private readonly ILogger<StorageController> _logger;

        public CUserId UserId => 
            new CUserId(new Guid(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value));



        public StorageController(
            IAuthServiceProxy authServiceProxy,
            ILoadsManager loadsManager,
            IStorageProvider storageProvider, 
            ILogger<StorageController> logger,
            ISignalsManager signalsManager)
        {
            _authServiceProxy = authServiceProxy;
            _signalsManager = signalsManager;
            _loadsManager = loadsManager;
            _storage = storageProvider;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("entities/{parentId}")]
        public ActionResult<List<CSharableEntityContract>> GetEntities(Guid parentId)
        {
            CStorageEntityId entityId = new CStorageEntityId(parentId);
            CUserId userId = UserId;

            List<CSharableEntity> entities = _storage.Entities.GetOwnerChildren(entityId, userId);

            return ContractsConverter.Convert(entities);
        }

        [Authorize]
        [HttpGet("shared_entities/{parentId}")]
        public ActionResult<List<CSharableEntityContract>> GetSharedEntities(Guid parentId)
        {
            CStorageEntityId entityId = new CStorageEntityId(parentId);
            CUserId userId = UserId;

            List<CSharableEntity> sharedEntities = _storage.Entities.GetSharedChildren(entityId, userId);

            return ContractsConverter.Convert(sharedEntities);
        }

        [Authorize]
        [HttpGet("entities/search/{parentId}/{pattern}")]
        public ActionResult<List<CSearchResultContract>> SearchEntities(Guid parentId, String pattern)
        {
            CStorageEntityId entityId = new CStorageEntityId(parentId);
            CUserId userId = UserId;

            List<CSearchResult> entities = _storage.Entities.SearchOwnByName(pattern, entityId, userId);

            return ContractsConverter.Convert(entities);
        }

        [Authorize]
        [HttpGet("shared_entities/search/{parentId}/{pattern}")]
        public ActionResult<List<CSearchResultContract>> SearchSharedEntities(Guid parentId, String pattern)
        {
            CStorageEntityId entityId = new CStorageEntityId(parentId);
            CUserId userId = UserId;

            List<CSearchResult> entities = _storage.Entities.SearchSharedByName(pattern, entityId, userId);

            return ContractsConverter.Convert(entities);
        }
        
        [Authorize]
        [HttpGet("upload_endpoint")]
        public ActionResult<CTransmissionEndPointContract> GetEndPointToUploadFile() 
        {
            return _loadsManager.EndPointToUploadToServer;
        }

        [Authorize]
        [HttpGet("download_endpoint")]
        public  ActionResult<CTransmissionEndPointContract> GetEndPointToLoadFile()
        {
            return _loadsManager.EndPointToDownloadFromServer;
        }

        [Authorize]
        [HttpGet("file/{id}")]
        public ActionResult<CSharableEntityContract> GetFile(Guid id)
        {
            CStorageEntityId fileId = new CStorageEntityId(id);
            CUserId userId = UserId;
            CSharableEntity file;
            try
            {
                file = _storage.Validator.EntityExists(fileId).HasReadAccess(userId)
                        .ToSharableEntity(userId);
            }
            catch (InvalidArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (AccessViolationException e)
            {
                return Unauthorized(e.Message);
            }
            
            return ContractsConverter.Convert(file);
        }

        [Authorize]
        [HttpPut("share")]
        public async Task<ActionResult> UpdateShare(CNewShareContract newShare)
        {
            CStorageEntityId entityId = new CStorageEntityId(newShare.EntityId);
            CUserId userId = UserId;
            try
            {
                _storage.Validator.EntityExists(entityId).IsOwner(userId);
            }
            catch (InvalidArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (AccessViolationException e)
            {
                return Unauthorized(e.Message);
            }

            CUserId userToShareId;
            try
            {
                CUserContract userToShare = await _authServiceProxy.GetUserWithName(newShare.UserName);
                userToShareId = new CUserId(userToShare.Id);
            }
            catch (NoSuchUserException e)
            {
                return BadRequest(e.Message);
            }
            
            _storage.Shares.SaveShare(entityId, userToShareId, newShare.AccessType);

            return Ok();
        }

        [Authorize]
        [HttpPost("dir")]
        public ActionResult<CSharableEntityContract> CreateDirectory(CNewDirContract dirInfo)
        {
            CStorageEntityId parentEntityId = new CStorageEntityId(dirInfo.ParentId);
            CUserId userId = UserId;
            String dirName = dirInfo.Name;
            IStorageEntity parent;
            try
            {
                parent = _storage.Validator.EntityExists(parentEntityId).HasWriteAccess(userId).ToEntity();
                _storage.Validator.EntityNotExists(parentEntityId, userId, dirName);
            }
            catch (InvalidArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (AccessViolationException e)
            {
                return Unauthorized(e.Message);
            }

            CSharableEntity createdDir = _storage.Entities.AddDir(parent, userId, dirName);

            _signalsManager.SignalEntityCreation(createdDir.AccessWrapper.Entity.Id, userId);
            _signalsManager.SignalStatsUpdate(userId);

            _logger.LogInformation($"User{userId} added dir #{createdDir.AccessWrapper.Entity.Id} (Owner {parent}).");

            return ContractsConverter.Convert(createdDir);
        }

        [Authorize]
        [HttpDelete("entity")]
        public ActionResult DeleteEntity([FromBody] Guid id)
        {
            CStorageEntityId entityId = new CStorageEntityId(id);
            CUserId userId = UserId;
            try
            {
                _storage.Validator.EntityExists(entityId).IsNotRoot().IsOwner(userId);
            }
            catch (InvalidArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (AccessViolationException e)
            {
                return Unauthorized(e.Message);
            }

            _signalsManager.SignalEntityDeletion(entityId, userId);
            
            _storage.Entities.DeleteEntity(entityId);

            _signalsManager.SignalStatsUpdate(userId);

            _logger.LogInformation($"User '{userId}' deleted entity #{entityId}.");

            return Ok();
        }
        

        [Authorize]
        [HttpPut("entity")]
        public ActionResult RenameEntity(CRenameEntityContract renameContract)
        {
            CStorageEntityId entityId = new CStorageEntityId(renameContract.EntityId);
            CUserId userId = UserId;
            String newName = renameContract.NewName;
            try
            {
                _storage.Validator.EntityExists(entityId).IsNotRoot().IsOwner(userId);
            }
            catch (InvalidArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (AccessViolationException e)
            {
                return Unauthorized(e.Message);
            }

            _storage.Entities.RenameEntity(entityId, newName);

            _signalsManager.SignalEntityRenaming(entityId, newName, userId);
            _signalsManager.SignalStatsUpdate(userId);

            _logger.LogInformation($"User '{UserId}' renamed entity #{entityId} to '{newName}'.");

            return Ok();
        }

        [Authorize]
        [HttpPost("copy")]
        public ActionResult CopyEntity(CCopyEntityContract copyContract)
        {
            CStorageEntityId entityId = new CStorageEntityId(copyContract.EntityId);
            CStorageEntityId newParentId = new CStorageEntityId(copyContract.NewParentId);
            CUserId userId = UserId;
            IStorageEntity parent;
            try
            {
                CSharableEntity entity = _storage.Validator.EntityExists(entityId).HasReadAccess(userId)
                    .ToSharableEntity(userId);
                parent = _storage.Validator.EntityExists(newParentId).IsDir().HasWriteAccess(userId)
                    .ToEntity();
                _storage.Validator.EntityNotExists(newParentId, userId, entity.AccessWrapper.Entity.Name);
            }
            catch (InvalidArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (AccessViolationException e)
            {
                return Unauthorized(e.Message);
            }

            CStorageEntityId id = _storage.Entities.CopyEntity(entityId, userId, parent).Result;

            _signalsManager.SignalEntityCreation(id, userId);
            _signalsManager.SignalStatsUpdate(userId);

            return Ok();
        }
    }
}