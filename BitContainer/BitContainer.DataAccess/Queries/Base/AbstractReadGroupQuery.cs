using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;

namespace BitContainer.DataAccess.Queries.Base
{
    public abstract class AbstractReadGroupQuery<TKey, TValue> : ISqlQuery<Dictionary<TKey, List<TValue>>>
    {
        public IMapper<TKey> KeyMapper { get; set; }
        public IMapper<TValue> ValueMapper { get; set; }

        protected AbstractReadGroupQuery(IMapper<TKey> keyMapper, IMapper<TValue> valueMapper)
        {
            KeyMapper = keyMapper;
            ValueMapper = valueMapper;
        }

        public abstract SqlCommand Prepare(SqlCommand command);

        public Dictionary<TKey, List<TValue>> Execute(SqlCommand command)
        {
            Dictionary<TKey, List<TValue>> result = new Dictionary<TKey, List<TValue>>();

            SqlCommand preparedCommand = Prepare(command);

            using (SqlDataReader reader = preparedCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    TKey key = KeyMapper.ReadItem(reader);
                    TValue value = ValueMapper.ReadItem(reader);

                    if (!result.ContainsKey(key))
                        result[key] = new List<TValue>();

                    result[key].Add(value);
                }
            }
            
            preparedCommand.Parameters.Clear();
            command.CommandType = CommandType.Text;

            return result;
        }
    }
}
