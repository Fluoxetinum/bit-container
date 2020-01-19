using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Mappers
{
    public class CRestrictedDirectoryMapper : IMapper<CRestrictedStorageEntity>
    {
        public CRestrictedStorageEntity ReadItem(SqlDataReader rd)
        {
            CDirectoryMapper mapper = new CDirectoryMapper();
            CDirectory dir = mapper.ReadItem(rd);

            ERestrictedAccessType restrictedAccess = rd.GetString("AccessType").ToAccessType();

            return new CRestrictedStorageEntity(dir, restrictedAccess);
        }
    }
}
