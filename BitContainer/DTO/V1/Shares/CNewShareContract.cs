using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1.Shares;

namespace BitContainer.Contracts.V1
{
    public class CNewShareContract
    {
        public String UserName { get; set; } // TODO: Search users on client (Andrey Gurin)
        public ERestrictedAccessTypeContract AccessTypeContract { get; set; }
        public IStorageEntityContract StorageEntity { get; set; }

        public static CNewShareContract Create(String userName, ERestrictedAccessTypeContract access, IStorageEntityContract storageEntity)
        {
            return new CNewShareContract()
            {
                UserName = userName,
                AccessTypeContract = access,
                StorageEntity = storageEntity
            };
        }
    }
}
