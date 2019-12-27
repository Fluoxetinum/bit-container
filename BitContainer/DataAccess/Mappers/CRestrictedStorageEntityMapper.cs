using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers
{
    public class CRestrictedStorageEntityMapper : IMapper<CRestrictedStorageEntity>
    {
        public static readonly CRestrictedDirectoryMapper DirMapper = new CRestrictedDirectoryMapper();
        public static readonly CRestrictedFileMapper FileMapper = new CRestrictedFileMapper();

        public CRestrictedStorageEntity ReadItem(SqlDataReader rd)
        {
            Int32 dataIndex = rd.GetOrdinal("Size");

            if (rd.IsDBNull(dataIndex))
                return DirMapper.ReadItem(rd);
            else
                return FileMapper.ReadItem(rd);
        }
    }
}
