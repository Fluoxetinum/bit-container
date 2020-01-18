using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;

namespace BitContainer.DataAccess.Queries.Base
{
    public abstract class AbstractReadQuery<T> : ISqlQuery<List<T>>
    {
        public IMapper<T> Mapper { get; set; }

        public AbstractReadQuery(IMapper<T> mapper)
        {
            Mapper = mapper;
        }

        public abstract SqlCommand Prepare(SqlCommand command);

        public List<T> Execute(SqlCommand command)
        {
            List<T> result = new List<T>();

            SqlCommand preparedCommand = Prepare(command);

            using (SqlDataReader reader = preparedCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(Mapper.ReadItem(reader));
                }
            }
            
            preparedCommand.Parameters.Clear();
            command.CommandType = CommandType.Text;

            return result;
        }
    }

}
