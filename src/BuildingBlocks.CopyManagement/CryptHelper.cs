using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.CopyManagement
{
    public static class CryptHelper
    {
        public static string ToFingerPrintMd5Hash(this string value)
        {
            var cryptoProvider = new MD5CryptoServiceProvider();
            var encoding = new ASCIIEncoding();
            var encodedBytes = encoding.GetBytes(value);
            var hashBytes = cryptoProvider.ComputeHash(encodedBytes);

            var hexString = BytesToHexString(hashBytes);
            return hexString;
        }

        private static string BytesToHexString(byte[] bytes)
        {
            var result = string.Empty;
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                var n = (int)b;
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;

                if (n2 > 9)
                {
                    result += ((char)(n2 - 10 + 'A')).ToString();
                }
                else
                {
                    result += n2.ToString();
                }

                if (n1 > 9)
                {
                    result += ((char)(n1 - 10 + 'A')).ToString();
                }
                else
                {
                    result += n1.ToString();
                }

                if ((i + 1) != bytes.Length && (i + 1) % 2 == 0)
                {
                    result += "-";
                }
            }
            return result;
        }
    }
}