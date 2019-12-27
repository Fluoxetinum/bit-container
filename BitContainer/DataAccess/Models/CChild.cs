using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CChild
    {
        public Guid Id { get; set; }
        public EEntityType Type { get; set; }
        public Int32 Level { get; set; }
    }
}
