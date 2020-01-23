using System.Collections.Generic;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Shared.Models;

namespace BitContainer.Contracts.V1.Shares
{
    public class CAccessWrapperContract
    {
        public CStorageEntityContract EntityContract { get; set; }
        public EAccessType Access { get; set; }
        
        public CAccessWrapperContract(){}

        public CAccessWrapperContract(CStorageEntityContract entityContract, EAccessType access)
        {
            EntityContract = entityContract;
            Access = access;
        }

    }
}
