using System;
using BitContainer.Contracts.V1.Storage;

namespace BitContainer.Contracts.V1.Auth
{
    public class CAuthenticatedUserContract
    {
        public String Token { get; set; }
        public CUserContract User { get; set; }
        public CStatsContract Stats { get; set; }

        public CAuthenticatedUserContract (String token, Guid id, String name, CStatsContract stats = null)
        {
            User = new CUserContract(id, name);
            Stats = stats;
            Token = token;
        }
    }
}
