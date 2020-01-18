using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Helpers
{
    public static class Extensions
    {
        public static Object GetDbNullIfEmpty(this String str)
        {
            if (String.IsNullOrEmpty(str))
                return DBNull.Value;

            return str;
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

        public static Guid GetGuid(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);

            if (rd.IsDBNull(id))
                return Guid.Empty;

            Guid value = rd.GetGuid(id);
            return value;

        }

        public static Byte[] GetBytes(this SqlDataReader rd, String columnName, Int32 size)
        {
            Byte[] data = new Byte[size];
            Int32 dataId = rd.GetOrdinal(columnName);
            rd.GetBytes(dataId, 0, data, 0, size);
            return data;
        }

        public static Boolean GetNumericBoolean(this SqlDataReader rd, String columnName)
        {
            Int32 id = rd.GetOrdinal(columnName);
            Int32 value = rd.GetInt32(id);
            return value == 1;
        }

        public static Boolean IsRootDir(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        public static Boolean NoReadAccess(this ERestrictedAccessType access)
        {
            return access != ERestrictedAccessType.Read && access != ERestrictedAccessType.Write;
        }

        public static Boolean NoWriteAccess(this ERestrictedAccessType access)
        {
            return  access != ERestrictedAccessType.Write;
        }

    }
}
