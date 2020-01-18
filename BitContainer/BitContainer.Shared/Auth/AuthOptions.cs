using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BitContainer.Shared.Auth
{
    public class AuthOptions
    {
        public const String Issuer = "MyAuthServer";
        public const String Audience = "http://localhost:53833";
        private const String Key = "mysupersecret_secretkey!1234";
        public static readonly TimeSpan LifeTime = TimeSpan.FromHours(1);

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
