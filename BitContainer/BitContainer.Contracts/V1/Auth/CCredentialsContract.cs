using System;

namespace BitContainer.Contracts.V1.Auth
{
    public class CCredentialsContract
    {
        public String UserName { get; set; }
        public Byte[] Hash { get; set; }

        public static CCredentialsContract Create(String userName, Byte[] hash)
        {
            return new CCredentialsContract()
            {
                UserName = userName,
                Hash = hash
            };
        }
    }
}
