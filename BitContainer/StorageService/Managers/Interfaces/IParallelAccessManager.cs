using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitContainer.StorageService.Managers.Interfaces
{
    public interface IParallelAccessManager
    {
        void StartReadFile(Guid id);
        void EndReadFile(Guid id);
        void StartWriteFile(Guid id);
        void EndWriteFile(Guid id);
    }
}
