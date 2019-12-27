using System.Collections.Generic;
using BitContainer.Contracts.V1.Shares;

namespace BitContainer.Contracts.V1.Storage
{
    public class COwnStorageEntitiesListContract
    {
        public List<COwnStorageEntityContract> Entities { get; set; }

        public COwnStorageEntitiesListContract(){}

        public COwnStorageEntitiesListContract(List<COwnStorageEntityContract> list)
        {
            Entities = list;
        }
    }
}
