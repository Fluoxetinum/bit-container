using System;
using System.Collections.Generic;
using BitContainer.Contracts.V1.Shares;

namespace BitContainer.Contracts.V1.Storage
{
    public class CSearchResultContract
    {
        public CSharableEntityContract SharableEntity { get; set; }
        public List<Guid> Parents { get; set; }

        public CSearchResultContract(CSharableEntityContract sharableEntity, List<Guid> parents)
        {
            SharableEntity = sharableEntity;
            Parents = parents;
        }
    }
}
