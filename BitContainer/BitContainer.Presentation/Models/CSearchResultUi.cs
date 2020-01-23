using System.Collections.Generic;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public class CSearchResultUi : ISharableEntityUi
    {
        public List<CStorageEntityId> Parents { get; set; }
        public ISharableEntityUi SharableEntity { get; set; }
        
        public IStorageEntityUi Entity
        {
            get => SharableEntity.Entity;
            set => SharableEntity.Entity = value;
        }

        public EAccessType Access
        {
            get => SharableEntity.Access;
            set => SharableEntity.Access = value;
        }

        public List<CShareUi> Shares
        {
            get => SharableEntity.Shares;
            set => SharableEntity.Shares = value;
        }

        public CSearchResultUi(ISharableEntityUi sharableEntity, List<CStorageEntityId> parents)
        {
            SharableEntity = sharableEntity;
            Parents = parents;
        }

    }
}
