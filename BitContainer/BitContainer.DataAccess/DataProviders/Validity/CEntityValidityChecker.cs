using System;
using System.Collections.Generic;
using System.Linq;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;
using Microsoft.SqlServer.Management.Common;

namespace BitContainer.DataAccess.DataProviders.Validity
{
      public class CEntityValidityChecker : IEntityValidityChecker
      {
            private readonly IStorageEntity _entity;
            private readonly List<CShare> _shares;

            public CEntityValidityChecker(IStorageEntity entity, List<CShare> shares)
            {
                _entity = entity;
                _shares = shares;
            }

            public IEntityValidityChecker IsNotRoot()
            {
                if (_entity.Id.IsRootId)
                    throw new InvalidArgumentException("Operation is not allowed for root.");
                return this;
            }

            public IEntityValidityChecker IsDir()
            {
                if (!(_entity is CDirectory))
                    throw new InvalidArgumentException("Operation is allowed only for directories.");
                return this;
            }

            public IEntityValidityChecker IsOwner(CUserId userId)
            {
                if (!(_entity.OwnerId == userId) 
                    && !_entity.Id.IsRootId)
                    throw new AccessViolationException($"User #{userId} does not have access to #{_entity.Id}");
                return this;
            }

            public IEntityValidityChecker HasReadAccess(CUserId userId)
            {
                CShare share = _shares.SingleOrDefault(s => s.UserId == userId && s.AccessType >= EAccessType.Read);
                if (share == null
                    && !(_entity.OwnerId == userId)
                    && !_entity.Id.IsRootId)
                    throw new AccessViolationException($"User #{userId} does not have access to #{_entity.Id}");
                return this;
            }

            public IEntityValidityChecker HasWriteAccess(CUserId userId)
            {
                CShare share = _shares.SingleOrDefault(s => s.UserId == userId && s.AccessType >= EAccessType.Read);
                if (share == null 
                    && !(_entity.OwnerId == userId)
                    && !_entity.Id.IsRootId)
                    throw new AccessViolationException($"User #{userId} does not have access to #{_entity.Id}");
                return this;
            }

            public CSharableEntity ToSharableEntity(CUserId userId)
            {
                if (_entity.Id.IsRootId)
                    throw new InvalidArgumentException("Creation of the root is not allowed.");

                bool isOwner = _entity.OwnerId == userId;
                
                if (isOwner) return new CSharableEntity(new CAccessWrapper(_entity, EAccessType.Write), _shares);
                
                CShare share = _shares.SingleOrDefault(s => s.UserId == userId);

                if (share == null)
                    throw new AccessViolationException($"User #{userId} does not have access to #{_entity.Id}");
                
                return new CSharableEntity(new CAccessWrapper(_entity, share.AccessType), _shares);
            }

            public IStorageEntity ToEntity()
            {
                return _entity;
            }
      }
}
