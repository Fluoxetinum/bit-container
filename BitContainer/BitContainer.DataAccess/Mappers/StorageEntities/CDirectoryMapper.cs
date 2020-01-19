using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CDirectoryMapper : IMapper<CDirectory>
    {
        public CDirectory ReadItem(SqlDataReader rd)
        {
            Guid id = rd.GetGuid("ID");
            Guid parentId = rd.GetGuid("ParentID");
            Guid ownerId = rd.GetGuid("OwnerID");
            String name = rd.GetString("Name");
            DateTime created = rd.GetDateTime("Created");
            
            return new CDirectory(id, parentId, ownerId, name, created);
        }
    }
}
