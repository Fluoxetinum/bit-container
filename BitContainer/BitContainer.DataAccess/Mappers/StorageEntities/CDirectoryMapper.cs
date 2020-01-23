using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CDirectoryMapper : IMapper<CDirectory>
    {
        public CDirectory ReadItem(SqlDataReader rd)
        {
            CStorageEntityId id = rd.GetStorageEntityId(DbNames.Entities.Id);
            CStorageEntityId parentId = rd.GetStorageEntityId(DbNames.Entities.ParentId);
            CUserId ownerId = rd.GetUserId(DbNames.Entities.OwnerId);
            String name = rd.GetString(DbNames.Entities.Name);
            DateTime created = rd.GetDateTime(DbNames.Entities.Created);
            
            return new CDirectory(id, parentId, ownerId, name, created);
        }
    }
}
