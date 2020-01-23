using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Helpers
{
    public static class SqlDataReaderEx
    {
        public static Boolean GetBoolean(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);
            Boolean value = rd.GetBoolean(id);
            return value;
        }

        public static Int32 GetInt32(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);
            Int32 value = rd.GetInt32(id);
            return value;
        }

        public static Int64 GetInt64(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);
            Int64 value = rd.GetInt64(id);
            return value;
        }

        public static DateTime GetDateTime(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);
            DateTime value = rd.GetDateTime(id);
            return value;
        }

        public static String GetString(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);
            String value = rd.GetString(id);
            return value;
        }

        public static EAccessType GetAccessType(this SqlDataReader rd, String columnName)
        {
            EAccessType access = (EAccessType) rd.GetInt32(columnName);
            if (!Enum.IsDefined(typeof(EAccessType), access))
                throw new InvalidCastException($"{nameof(access)} has invalid int value.");
            return access;
        }

        public static CUserId GetUserId(this SqlDataReader rd, String columnName)
        {
            Int32 index = rd.GetOrdinal(columnName);
            Guid id = rd.GetGuid(index);
            return new CUserId(id);
        }

        public static CStorageEntityId GetStorageEntityId(this SqlDataReader rd, String columnName)
        {
            Int32 index = rd.GetOrdinal(columnName);

            if (rd.IsDBNull(index))
                return CStorageEntityId.RootId;

            Guid id = rd.GetGuid(index);
            return new CStorageEntityId(id);
        }

        public static Byte[] GetBytes(this SqlDataReader rd, String columnName, Int32 size)
        {
            Byte[] data = new Byte[size];
            Int32 dataId = rd.GetOrdinal(columnName);
            rd.GetBytes(dataId, 0, data, 0, size);
            return data;
        }
    }
}
