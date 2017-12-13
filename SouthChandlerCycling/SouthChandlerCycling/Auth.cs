using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SouthChandlerCycling.Models;

namespace SouthChandlerCycling
{
    public class Auth
    {
        private static string secret = "Badges?Wedontneednostinkinbadges!!!PavioWasHere!!!";

        public static string GenerateJWT(User user)
        {
            var payload = new Dictionary<string, object>
            {
                {"UserName", user.UserName },
                {"Role", user.Role }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }
        public static bool IsValidToken(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);

                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
        public static string GetPasswordString(string token)
        {
            string result;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                result = decoder.Decode(token, secret, verify: true);
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }
            public static string Hash(string Password, string Salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, Encoding.ASCII.GetBytes(Salt), 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            return Convert.ToBase64String(hash);
        }

        public static string GenerateSalt()
        {
            return Convert.ToBase64String(GenerateSaltBytes());
        }

        private static byte[] GenerateSaltBytes()
        {
            // returns a randomly generated Salt...
            byte[] Salt;

            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(Salt = new byte[16]);
            return Salt;
        }
    }
}
