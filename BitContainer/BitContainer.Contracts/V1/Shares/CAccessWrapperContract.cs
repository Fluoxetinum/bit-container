using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1.Storage;

namespace BitContainer.Contracts.V1.Shares
{
    public class CAccessWrapperContract
    {
        public CStorageEntityContract EntityContract { get; set; }
        public ERestrictedAccessTypeContract Access { get; set; }
        public Boolean HasShares { get; set; }

        public CAccessWrapperContract(CStorageEntityContract entityContract,
            ERestrictedAccessTypeContract access,
            Boolean hasShares)
        {
            EntityContract = entityContract;
            Access = access;
            HasShares = hasShares;
        }

    }
}
