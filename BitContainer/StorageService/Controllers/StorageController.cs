using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.Shared.Auth;
using BitContainer.Shared.Http;
using BitContainer.Shared.Http.Exceptions;
using BitContainer.StorageService.Helpers;
using BitContainer.StorageService.Managers;
using BitContainer.StorageService.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BitContainer.StorageService.Controllers
{
    [Route("storage")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly ILoadsManager _loadsManager;
        private readonly IStorageProvider _storage;
        private readonly ILogger<StorageController> _logger;

        public Guid UserId => new Guid(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

        public StorageController(
            ILoadsManager loadsManager,
            IStorageProvider storageProvider, 
            ILogger<StorageController> logger)
        {
            _loadsManager = loadsManager;
            _storage = storageProvider;
            _logger = logger;
        }
        
        [Authorize]
        [HttpGet("entities/{parentId}")]
        public ActionResult<COwnStorageEntitiesListContract> GetStorageEntities(Guid parentId)
        {
            List<COwnStorageEntity> entities = _storage.StorageEntities.GetOwnerChildren(parentId, UserId);
            return ContractsConverter.ToOwnStorageEntitiesListContract(entities);
        }

        [Authorize]
        [HttpGet("shared_entities/{parentId}")]
        public ActionResult<CRestrictedStorageEntitiesListContract> GetSharedStorageEntities(Guid parentId)
        {
            List<CRestrictedStorageEntity> sharedEntities = _storage.StorageEntities.GetSharedChildren(parentId, UserId);
            return ContractsConverter.ToRestrictedEntitiesListContract(sharedEntities);
        }

        [Authorize]
        [HttpGet("entities/search/{parentId}/{pattern}")]
        public ActionResult<List<CSearchResultContract>> SearchStorageEntities(Guid parentId, String pattern)
        {
            List<CSearchResult> entities = _storage.SearchOwnEntitiesByName(pattern, parentId, UserId);
            return ContractsConverter.ToSearchResultContractList(entities);
        }

        [Authorize]
        [HttpGet("shared_entities/search/{parentId}/{pattern}")]
        public ActionResult<List<CSearchResultContract>> SearchSharedStorageEntities(Guid parentId,
            String pattern)
        {
            List<CSearchResult> entities = _storage.SearchRestrictedEntitiesByName(pattern, parentId, UserId);
            return ContractsConverter.ToSearchResultContractList(entities);
        }
        
        [Authorize]
        [HttpGet("file/{id}")]
        public ActionResult<IAccessWrapperContract> GetFile(Guid id)
        {
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(id, UserId);
            if (restrictedAccess.NoReadAccess()) return Unauthorized();

            CFile file = _storage.StorageEntities.GetFile(id);
            if (file == null) return BadRequest("No such file");

            if (file.OwnerId == UserId)
            {
                Boolean isShared = _storage.Shares.IsStorageEntityHasShare(file.Id);
                return ContractsConverter.ToOwnStorageEntityContract(file, isShared);
            }

            return ContractsConverter.ToRestrictedStorageEntityContract(file, restrictedAccess);
        }

        [Authorize]
        [HttpPut("share")]
        public async Task<ActionResult> UpdateShare(CNewShareContract newShare)
        {
            CUserContract user = null;
            try
            {
                user = await AuthServiceProxy.GetUserWithName(newShare.UserName);
            }
            catch (NoSuchUserException e)
            {
                return BadRequest(e.Message);
            }
            
            ERestrictedAccessType restrictedAccessToShare = newShare.AccessTypeContract.ToAccessType();

            Guid fileOwnerId = _storage.Shares.GetStorageEntityOwner(newShare.StorageEntity.Id);
            if (fileOwnerId != UserId) return Unauthorized();
            
            CShare fShare = _storage.Shares.GetStorageEntityShare(user.Id, newShare.StorageEntity.Id);
            
            if (fShare != null)
                _storage.Shares.UpdateStorageEntityShare(user.Id, restrictedAccessToShare, newShare.StorageEntity.Id);
            else
                _storage.Shares.AddStorageEntityShare(user.Id, restrictedAccessToShare, newShare.StorageEntity.Id);
                    
            return Ok();
        }

        [Authorize]
        [HttpPost("dir")]
        public ActionResult<IAccessWrapperContract> AddDirectory(CDirectoryContract dirInfo)
        {
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(dirInfo.ParentId, UserId);
            if (restrictedAccess.NoWriteAccess()) return Unauthorized();

            CDirectory dir = _storage.StorageEntities.GetDir(dirInfo.ParentId, userId:UserId, dirInfo.Name);
            if (dir != null) return BadRequest("Such dir already exists");

            CDirectory createdDir = _storage.StorageEntities.AddDir(dirInfo.ParentId, UserId, dirInfo.Name);

            _logger.LogInformation($"User{UserId} added dir #{dirInfo.Id} (Owner {createdDir.OwnerId}).");

            if (createdDir.OwnerId == UserId)
            {
                Boolean isShared = _storage.Shares.IsStorageEntityHasShare(createdDir.Id);
                return ContractsConverter.ToOwnStorageEntityContract(createdDir, isShared);
            }

            return ContractsConverter.ToRestrictedStorageEntityContract(createdDir, restrictedAccess);
        }

        [Authorize]
        [HttpDelete("dir")]
        public ActionResult DeleteDirectory(CDirectoryContract dirContract)
        {
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(dirContract.Id, UserId);
            if (restrictedAccess.NoWriteAccess()) return Unauthorized();
            
            _storage.StorageEntities.DeleteDir(dirContract.Id);

            _logger.LogInformation($"User '{UserId}' deleted dir #{dirContract.Id}.");

            return Ok();
        }
        
        [Authorize]
        [HttpDelete("file")]
        public ActionResult DeleteFile(CFileContract fileContract)
        {
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(fileContract.Id, UserId);
            if (restrictedAccess.NoWriteAccess()) return Unauthorized();
            
            _storage.StorageEntities.DeleteFile(fileContract.Id);

            _logger.LogInformation($"User '{UserId}' deleted file #{fileContract.Id}.");

            return Ok();
        }

        [Authorize]
        [HttpPut("dir")]
        public ActionResult RenameDirectory(CDirectoryContract dirContract)
        {
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(dirContract.Id, UserId);
            if (restrictedAccess.NoWriteAccess()) return Unauthorized();

            _storage.StorageEntities.RenameEntity(dirContract.Id, dirContract.Name);

            _logger.LogInformation($"User '{UserId}' renamed dir #{dirContract.Id} to '{dirContract.Name}'.");

            return Ok();
        }

        [Authorize]
        [HttpPut("file")]
        public ActionResult RenameFile(CFileContract fileContract)
        {
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(fileContract.Id, UserId);
            if (restrictedAccess.NoWriteAccess()) return Unauthorized();

            _storage.StorageEntities.RenameEntity(fileContract.Id, fileContract.Name);

            _logger.LogInformation($"User '{UserId}' renamed file #{fileContract.Id} to '{fileContract.Name}'.");

            return Ok();
        }

        [Authorize]
        [HttpPost("file/copy")]
        public ActionResult CopyFile(CFileContract fileContract)
        {
            Guid fileId = fileContract.Id;
            Guid newParentId = fileContract.ParentId;
            
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(fileId, UserId);
            if (restrictedAccess.NoReadAccess()) return Unauthorized();

            ERestrictedAccessType destinationAccess = _storage.Shares.CheckStorageEntityAccess(newParentId, UserId);
            if (destinationAccess.NoWriteAccess()) return Unauthorized();

            _storage.StorageEntities.CopyFile(fileId,UserId, newParentId);

            return Ok();
        }

        [Authorize]
        [HttpPost("dir/copy")]
        public ActionResult CopyDir(CDirectoryContract dirContract)
        {
            Guid dirId = dirContract.Id;
            Guid newParentId = dirContract.ParentId;
            
            ERestrictedAccessType restrictedAccess = _storage.Shares.CheckStorageEntityAccess(dirId, UserId);
            if (restrictedAccess.NoReadAccess()) return Unauthorized();

            ERestrictedAccessType destinationAccess = _storage.Shares.CheckStorageEntityAccess(newParentId, UserId);
            if (destinationAccess.NoWriteAccess()) return Unauthorized();

            _storage.StorageEntities.CopyDir(dirId, UserId, newParentId);

            return Ok();
        }

        [Authorize]
        [HttpGet("upload_endpoint")]
        public ActionResult<CTransmissionEndPointContract> GetEndPointToUploadFile() 
        {
            return _loadsManager.GetEndPointToUpload();
        }

        [Authorize]
        [HttpGet("download_endpoint")]
        public  ActionResult<CTransmissionEndPointContract> GetEndPointToLoadFile()
        {
            return _loadsManager.GetEndPointToDownload();
        }
    }
}