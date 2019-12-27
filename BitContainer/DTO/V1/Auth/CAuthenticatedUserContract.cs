using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1.Auth;

namespace BitContainer.Contracts.V1
{
    public class CAuthenticatedUserContract
    {
        public String Token { get; set; }
        public CUserContract User { get; set; }
        public CStatsContract Stats { get; set; }

        public CAuthenticatedUserContract (String token, Guid id, String name, Int32 filesCount, Int32 dirsCount, Int32 storageSize)
        {
            User = new CUserContract(id, name);
            Stats = new CStatsContract(filesCount, dirsCount, storageSize);
            Token = token;
        }
    }
}
