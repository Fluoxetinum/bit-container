using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Mappers
{
    public class COwnFileMapper : IMapper<COwnStorageEntity>
    {
        public COwnStorageEntity ReadItem(SqlDataReader rd)
        {
            CFileMapper m = new CFileMapper();
            CFile file = m.ReadItem(rd);

            Boolean isShared = rd.GetNumericBoolean("IsShared");

            return new COwnStorageEntity(file, isShared);
        }
    }
}
