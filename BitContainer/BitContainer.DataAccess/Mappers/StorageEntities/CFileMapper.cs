using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CFileMapper : IMapper<CFile>
    {
        public CFile ReadItem(SqlDataReader rd)
        {
            CStorageEntityId id = rd.GetStorageEntityId(DbNames.Entities.Id);
            CStorageEntityId parentId = rd.GetStorageEntityId(DbNames.Entities.ParentId);
            CUserId ownerId = rd.GetUserId(DbNames.Entities.OwnerId);
            String name = rd.GetString(DbNames.Entities.Name);
            DateTime created = rd.GetDateTime(DbNames.Entities.Created);
            Int64 size = rd.GetInt64(DbNames.Entities.Size);

            return new CFile(id, parentId, ownerId, name, created, size);
        }
    }
}
