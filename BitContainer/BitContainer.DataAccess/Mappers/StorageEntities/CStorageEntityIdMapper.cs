using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CStorageEntityIdMapper : IMapper<CStorageEntityId>
    {
        public CStorageEntityId ReadItem(SqlDataReader rd)
        {
            return rd.GetStorageEntityId(DbNames.Entities.Id);
        }
    }
}
