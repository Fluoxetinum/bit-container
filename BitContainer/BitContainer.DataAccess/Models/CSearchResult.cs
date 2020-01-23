using System.Collections.Generic;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Models
{
    public class CSearchResult
    {
        public CSharableEntity SharableEntity { get; set; }
        public LinkedList<CStorageEntityId> Parents { get; set; }

        public CSearchResult(CSharableEntity sharableEntity, LinkedList<CStorageEntityId> parents)
        {
            SharableEntity = sharableEntity;
            Parents = parents;
        }
    }
}
