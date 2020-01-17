using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers
{
    public class CShareMapper : IMapper<CShare>
    {
        public CShare ReadItem(SqlDataReader rd)
        {
            Guid personId = rd.GetGuid("UserApprovedID");
            
            ERestrictedAccessType restrictedAccessType = rd.GetString("AccessType").ToAccessType();

            Guid storageEntityId = rd.GetGuid("StorageEntityId");

            return new CShare()
            {
                PersonId = personId,
                StorageEntityId = storageEntityId,
                RestrictedAccessType = restrictedAccessType
            };
        }
    }
}
