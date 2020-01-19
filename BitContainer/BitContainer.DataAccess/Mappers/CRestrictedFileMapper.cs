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
    public class CRestrictedFileMapper : IMapper<CRestrictedStorageEntity>
    {
        public CRestrictedStorageEntity ReadItem(SqlDataReader rd)
        {
            CFileMapper mapper = new CFileMapper();
            CFile file = mapper.ReadItem(rd);

            ERestrictedAccessType restrictedAccess = rd.GetString("AccessType").ToAccessType();
            
            return new CRestrictedStorageEntity(file, restrictedAccess);
        }
    }
}
