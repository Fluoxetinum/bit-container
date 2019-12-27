using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Shares
{
    public class CRestrictedStorageEntitiesListContract
    {
        public List<CRestrictedStorageEntityContract> Entites { get; set; }

        public CRestrictedStorageEntitiesListContract() {}

        public CRestrictedStorageEntitiesListContract(List<CRestrictedStorageEntityContract> list)
        {
            Entites = list;
        }
    }
}
