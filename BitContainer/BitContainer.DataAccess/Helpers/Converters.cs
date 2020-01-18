using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Helpers
{
    public static class Converters
    {
        public static ERestrictedAccessType ToAccessType(this String str)
        {
            ERestrictedAccessType type = 
                (ERestrictedAccessType) Enum.Parse(typeof(ERestrictedAccessType), str, ignoreCase:true);
            return type;
        }

        public static Int32 ToInt32(this ERestrictedAccessType type)
        {
            return (Int32) type;
        }

        public static EEntityType ToEntityType(this String str)
        {
            EEntityType type = 
                (EEntityType) Enum.Parse(typeof(EEntityType), str, ignoreCase:true);
            return type;
        }
    }
}
