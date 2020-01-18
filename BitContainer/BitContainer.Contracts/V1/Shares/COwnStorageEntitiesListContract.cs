using System.Collections.Generic;

namespace BitContainer.Contracts.V1.Shares
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
