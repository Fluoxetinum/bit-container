using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Shares
{
    public class COwnStorageEntityContract : IAccessWrapperContract
    {
        public IStorageEntityContract EntityContract { get; set; }
        public Boolean IsShared { get; set; }

        public COwnStorageEntityContract(){}

        public COwnStorageEntityContract(IStorageEntityContract entity, Boolean isShared)
        {
            EntityContract = entity;
            IsShared = isShared;
        }
    }
}
