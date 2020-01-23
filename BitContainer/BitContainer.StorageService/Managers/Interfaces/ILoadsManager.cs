using BitContainer.Contracts.V1;

namespace BitContainer.Service.Storage.Managers.Interfaces
{
    public interface ILoadsManager
    {
        CTransmissionEndPointContract EndPointToUploadToServer { get; }
        CTransmissionEndPointContract EndPointToDownloadFromServer { get; }
    }
}
