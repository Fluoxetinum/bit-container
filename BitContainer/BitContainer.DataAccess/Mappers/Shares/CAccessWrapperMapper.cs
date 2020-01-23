using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.Shares
{
    public class CAccessWrapperMapper : IMapper<CAccessWrapper>
    {
        readonly CStorageEntityMapper _entityMapper = new CStorageEntityMapper();

        public CAccessWrapper ReadItem(SqlDataReader rd)
        {
            IStorageEntity storageEntity = _entityMapper.ReadItem(rd);
            EAccessType acessType = rd.GetAccessType(DbNames.Shares.AccessTypeId);

            return new CAccessWrapper(storageEntity, acessType);
        }
    }
}
