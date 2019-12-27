using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CUserStats
    {
        public Guid UserId { get; set; }
        public Int32 FilesCount { get; set; }
        public Int32 DirsCount { get; set; }
        public Int32 StorageSize { get; set; }
    }
}
