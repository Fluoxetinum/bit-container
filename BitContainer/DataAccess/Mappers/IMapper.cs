using System.Data.Sql;
using System.Data.SqlClient;

namespace BitContainer.DataAccess.Mappers
{
    public interface IMapper<out T>
    {
        T ReadItem(SqlDataReader rd);
    }
}
