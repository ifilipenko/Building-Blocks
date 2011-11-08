using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Common.Utils
{
    public static class HashHelper
    {
        public static string ToMd5HashedString(this string value)
        {
            var crypt = MD5.Create();
            byte[] hash = crypt.ComputeHash(Encoding.Default.GetBytes(value));
            var builder = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}