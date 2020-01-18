using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1.Storage;

namespace BitContainer.Contracts.V1.Shares
{
    public class CRestrictedStorageEntityContract : IAccessWrapperContract
    {
        public IStorageEntityContract EntityContract { get; set; }
        public ERestrictedAccessTypeContract AccessContract { get; set; }

        public CRestrictedStorageEntityContract() {}

        public CRestrictedStorageEntityContract(IStorageEntityContract entityContract, ERestrictedAccessTypeContract accessContract)
        {
            EntityContract = entityContract;
            AccessContract = accessContract;
        }
    }
}
