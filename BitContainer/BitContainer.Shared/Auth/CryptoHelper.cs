using System;
using System.Security.Cryptography;
using System.Text;

namespace BitContainer.Shared.Auth
{
    public class CryptoHelper
    {
        private static readonly Byte[] StaticSalt = Encoding.UTF8.GetBytes("bit-container.org");

        private const Int32 SaltSize = 16;
        private const Int32 HashSize = 16;

        public static Byte[] GenerateSalt()
        {
            var rand = RandomNumberGenerator.Create();
            Byte[] salt = new byte[SaltSize];
            rand.GetBytes(salt);
            return salt;
        }

        public static Byte[] GenerateHashWithStaticSalt(String password)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, StaticSalt);
            Byte[] hash = rfc.GetBytes(HashSize);
            return hash;
        }

        public static Byte[] GenerateHash(Byte[] passwordHash, Byte[] salt)
        {
            String strPasswordHash = Encoding.UTF8.GetString(passwordHash);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(strPasswordHash, salt);
            Byte[] hash = rfc.GetBytes(HashSize);
            return hash;
        }

    }
}
