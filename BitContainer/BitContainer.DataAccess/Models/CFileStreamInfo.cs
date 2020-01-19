using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CFileStreamInfo 
    {
        public String Path { get; set; }
        public byte[] TransactionContext { get; set; }
    }
}
