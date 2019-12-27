using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers
{
    class CChildrenMapper : IMapper<CChild>
    {
        public CChild ReadItem(SqlDataReader rd)
        {
            Guid id = rd.GetGuid("ID");

            EEntityType type;
            Int32 dataIndex = rd.GetOrdinal("Size");
            if (rd.IsDBNull(dataIndex))
                type = EEntityType.Directory;
            else
                type = EEntityType.File;

            Int32 level = rd.GetInt32("Level");

            return new CChild()
            {
                Id = id,
                Type = type,
                Level = level
            };
        }
    }
}
