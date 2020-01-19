using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CStorageEntityMapper : IMapper<IStorageEntity>
    {
        private static readonly CDirectoryMapper _dirMapper = new CDirectoryMapper();
        private static readonly CFileMapper _fileMapper = new CFileMapper();

        public IStorageEntity ReadItem(SqlDataReader rd)
        {
            Int32 dataIndex = rd.GetOrdinal("Size");

            if (rd.IsDBNull(dataIndex))
                return _dirMapper.ReadItem(rd);
            else
                return _fileMapper.ReadItem(rd);
        }
    }
}
