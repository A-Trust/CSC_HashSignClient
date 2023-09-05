using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HashSignExampleClient.classes
{
    internal class TokenGenerator
    {

        private static readonly Func<byte[], byte[], byte[]> SignToken = (key, value) =>
        {
            using (var sha = new HMACSHA256(key))
            {
                return sha.ComputeHash(value);
            }
        };

        public static string GenerateToken(string subscriberId, string clientId, string clientSecret)
        {
            string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
            string payload = GeneratePayload(subscriberId, clientId);
            byte[] bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
            byte[] keyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(clientSecret));
            byte[] signature = SignToken(keyBytes, bytesToSign);
            string b64Signature = UrlEncode(signature);
            return string.Join(".", new List<string> { header, payload, b64Signature });
        }

        private static string GeneratePayload(string subscriberId, string clientId)
        {
            Int32 currentTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string jti = Guid.NewGuid().ToString();
            string name = "optional signature application name";
            string result = "{\"sub\":\"" + subscriberId + "\",\"iat\":" + currentTime + ",\"jti\":\"" + jti + "\",\"iss\":\"" + name + "\",\"azp\":\"" + clientId + "\"}";
            result = Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
            return result;
        }

        private static string UrlEncode(byte[] data)
        {
            string b64 = Convert.ToBase64String(data);
            string b64url = b64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return b64url;
        }
    }
}
