using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CShare
    {
        public Guid PersonId { get; set; }
        public Guid StorageEntityId { get; set; }
        public ERestrictedAccessType RestrictedAccessType { get; set; }
    }
}
