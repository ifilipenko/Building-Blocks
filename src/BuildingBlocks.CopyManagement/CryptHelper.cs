using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.CopyManagement
{
    public static class CryptHelper
    {
        public static string ToFingerPrintMd5Hash(this string value, char? separator = '-')
        {
            var cryptoProvider = new MD5CryptoServiceProvider();
            var encoding = new ASCIIEncoding();
            var encodedBytes = encoding.GetBytes(value);
            var hashBytes = cryptoProvider.ComputeHash(encodedBytes);

            var hexString = BytesToHexString(hashBytes, separator);
            return hexString;
        }

        private static string BytesToHexString(byte[] bytes, char? separator)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                var n = (int)b;
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;

                if (n2 > 9)
                {
                    stringBuilder.Append((char) n2 - 10 + 'A');
                }
                else
                {
                    stringBuilder.Append(n2);
                }

                if (n1 > 9)
                {
                    stringBuilder.Append((char) n1 - 10 + 'A');
                }
                else
                {
                    stringBuilder.Append(n1);
                }

                if (separator.HasValue && ((i + 1) != bytes.Length && (i + 1) % 2 == 0))
                {
                    stringBuilder.Append(separator.Value);
                }
            }
            return stringBuilder.ToString();
        }
    }
}