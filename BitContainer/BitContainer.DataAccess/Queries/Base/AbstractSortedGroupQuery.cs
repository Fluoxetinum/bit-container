using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;

namespace BitContainer.DataAccess.Queries.Base
{
    public abstract class AbstractSortedGroupQuery<TKey, TValue> : ISqlQuery<SortedDictionary<TKey, List<TValue>>>
    {
        public IMapper<TKey> KeyMapper { get; set; }
        public IMapper<TValue> ValueMapper { get; set; }
        public Comparer<TKey> Comparer { get; set; }

        protected AbstractSortedGroupQuery(IMapper<TKey> keyMapper, IMapper<TValue> valueMapper)
        {
            KeyMapper = keyMapper;
            ValueMapper = valueMapper;
            Comparer = Comparer<TKey>.Default;
        }

        public abstract SqlCommand Prepare(SqlCommand command);

        public SortedDictionary<TKey, List<TValue>> Execute(SqlCommand command)
        {
            SortedDictionary<TKey, List<TValue>> result = new SortedDictionary<TKey, List<TValue>>(Comparer);

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
