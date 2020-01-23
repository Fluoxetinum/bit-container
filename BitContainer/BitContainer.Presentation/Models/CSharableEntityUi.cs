using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public class CSharableEntityUi : ISharableEntityUi
    {
        public IStorageEntityUi Entity { get; set; }
        public EAccessType Access { get; set; }
        public List<CShareUi> Shares { get; set; }

        public CSharableEntityUi(IStorageEntityUi entity, EAccessType access, List<CShareUi> shares)
        {
            Entity = entity;
            Access = access;
            Shares = shares;
        }

    }
}
