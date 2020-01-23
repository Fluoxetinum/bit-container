using System;

namespace BitContainer.DataAccess.Models.StorageEntities
{
    public class CFileStreamInfo 
    {
        public String Path { get; set; }
        public byte[] TransactionContext { get; set; }
    }
}
