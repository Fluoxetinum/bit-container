using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;

namespace BitContainer.StorageService.Managers.Interfaces
{
    public interface ILoadsManager
    {
        CTransmissionEndPointContract GetEndPointToUpload();
        CTransmissionEndPointContract GetEndPointToDownload();
    }
}
