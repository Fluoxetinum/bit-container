using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitContainer.StorageService.Managers.Interfaces;

namespace BitContainer.StorageService.Managers
{
    public class CReadWriteFileLock : IParallelAccessManager
    {
        class DbRowRequestLock
        {
            public ReaderWriterLockSlim Lock { get; }
            public DateTime TimeStap { get; set; } 

            public DbRowRequestLock()
            {
                Lock = new ReaderWriterLockSlim();
                TimeStap = DateTime.Now;
            }

            public void UpdateTimeStamp()
            {
                TimeStap = DateTime.Now;
            }
        }

        private readonly ConcurrentDictionary<Guid, DbRowRequestLock> FileLocks;

        private DbRowRequestLock GetFileLock(Guid id)
        {
            var fileLock = FileLocks.GetOrAdd(id, fileId => new DbRowRequestLock());
            fileLock.UpdateTimeStamp();
            return fileLock;
        }

        public CReadWriteFileLock()
        {
            FileLocks = new ConcurrentDictionary<Guid, DbRowRequestLock>();
        }

        public void StartReadFile(Guid id)
        {
            DbRowRequestLock handle = GetFileLock(id);
            handle.Lock.EnterReadLock();
        }

        public void EndReadFile(Guid id)
        {
            DbRowRequestLock handle = GetFileLock(id);
            handle.Lock.ExitReadLock();
        }

        public void StartWriteFile(Guid id)
        {
            DbRowRequestLock handle = GetFileLock(id);
            handle.Lock.EnterWriteLock();
        }

        public void EndWriteFile(Guid id)
        {
            DbRowRequestLock handle = GetFileLock(id);
            handle.Lock.ExitWriteLock();
        }
    }
}
