using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;

namespace BitContainer.DataAccess.Queries.Base
{
    public abstract class AbstractReadDictionaryQuery<TKey, TValue> : ISqlQuery<Dictionary<TKey, TValue>>
    {
        public IMapper<TKey> KeyMapper { get; set; }
        public IMapper<TValue> ValueMapper { get; set; }

        protected AbstractReadDictionaryQuery(IMapper<TKey> keyMapper, IMapper<TValue> valueMapper)
        {
            KeyMapper = keyMapper;
            ValueMapper = valueMapper;
        }

        public abstract SqlCommand Prepare(SqlCommand command);

        public Dictionary<TKey, TValue> Execute(SqlCommand command)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();

            SqlCommand preparedCommand = Prepare(command);

            using (SqlDataReader reader = preparedCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    TKey key = KeyMapper.ReadItem(reader);
                    TValue value = ValueMapper.ReadItem(reader);
                    result.Add(key, value);
                }
            }
            
            preparedCommand.Parameters.Clear();
            command.CommandType = CommandType.Text;

            return result;
        }
    }
}
