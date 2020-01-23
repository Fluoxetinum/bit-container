using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public interface ISharableEntityUi
    {
        public IStorageEntityUi Entity { get; set; }
        public EAccessType Access { get; set; }
        public List<CShareUi> Shares { get; set; }
    }
}
