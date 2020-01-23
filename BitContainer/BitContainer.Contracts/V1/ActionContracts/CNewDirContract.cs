using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.ActionContracts
{
    public class CNewDirContract
    {
        public Guid ParentId { get; set; }
        public String Name { get; set; }

        public CNewDirContract(Guid parentId, String name)
        {
            ParentId = parentId;
            Name = name;
        }

    }
}
