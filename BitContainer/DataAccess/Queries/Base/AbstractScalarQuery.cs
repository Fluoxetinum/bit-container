using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;

namespace BitContainer.DataAccess.Queries.Base
{
    public abstract class AbstractScalarQuery<T> : IQuery<T> 
    {
        public IMapper<T> Mapper { get; set; }

        public AbstractScalarQuery(IMapper<T> mapper)
        {
            Mapper = mapper;
        }

        public abstract SqlCommand Prepare(SqlCommand command);

        public T Execute(SqlCommand command)
        {
            T result = default(T);

            SqlCommand preparedCommand = Prepare(command);

            using (SqlDataReader reader = preparedCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    result = Mapper.ReadItem(reader);
                }
            }

            preparedCommand.Parameters.Clear();
            command.CommandType = CommandType.Text;

            
            return result;
        }
    }
}
