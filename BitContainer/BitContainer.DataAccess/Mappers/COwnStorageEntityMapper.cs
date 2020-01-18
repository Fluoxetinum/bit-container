using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers
{
    public class COwnStorageEntityMapper : IMapper<COwnStorageEntity>
    {
        public static readonly COwnDirectoryMapper DirMapper = new COwnDirectoryMapper();
        public static readonly COwnFileMapper FileMapper = new COwnFileMapper();

        public COwnStorageEntity ReadItem(SqlDataReader rd)
        {
            Int32 dataIndex = rd.GetOrdinal("Size");

            if (rd.IsDBNull(dataIndex))
                return DirMapper.ReadItem(rd);
            else
                return FileMapper.ReadItem(rd);
        }
    }
}
