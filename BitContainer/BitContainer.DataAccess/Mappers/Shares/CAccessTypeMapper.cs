using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.Shares
{
    public class CAccessTypeMapper : IMapper<EAccessType>
    {
        public EAccessType ReadItem(SqlDataReader rd)
        {
            return rd.GetAccessType(DbNames.Shares.AccessTypeId);
        }
    }
}
