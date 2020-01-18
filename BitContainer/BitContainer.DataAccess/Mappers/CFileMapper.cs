using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Helpers;

namespace BitContainer.DataAccess.Mappers
{
    public class CFileMapper : IMapper<CFile>
    {
        public CFile ReadItem(SqlDataReader rd)
        {
            Guid id = rd.GetGuid("ID");
            Guid parentId = rd.GetGuid("ParentID");
            Guid ownerId = rd.GetGuid("OwnerID");
            String name = rd.GetString("Name");
            DateTime created = rd.GetDateTime("Created");
            Int32 size = rd.GetInt32("Size");

            return new CFile(id, parentId, ownerId, name, created, size);
        }
    }
}
