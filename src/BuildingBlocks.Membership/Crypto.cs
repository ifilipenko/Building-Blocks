using System;
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace BuildingBlocks.Membership
{
    public static class Crypto
    {
        private const int TokenSizeInBytes = 16;
        private const int Pbkdf2Count = 1000;
        private const int Pbkdf2SubkeyLength = 256 / 8;
        private const int SaltSize = 128 / 8;

        public static string GenerateSalt(int byteLength = SaltSize)
        {
            var Buff = new byte[byteLength];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetBytes(Buff);
            }
            return Convert.ToBase64String(Buff);
        }

        public static string Hash(string input, string algorithm = "sha256")
        {
            if (input == null)
                throw new ArgumentNullException("input");

            return Hash(Encoding.UTF8.GetBytes(input), algorithm);
        }

        public static string Hash(byte[] input, string algorithm = "sha256")
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var hashAlgorithm = HashAlgorithm.Create(algorithm))
            {
                if (hashAlgorithm == null)
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "not supported hash alg", algorithm));
                
                var hashData = hashAlgorithm.ComputeHash(input);
                return BinaryToHex(hashData);
            }
        }

        public static string SHA1(string input)
        {
            return Hash(input, "sha1");
        }

        public static string SHA256(string input)
        {
            return Hash(input, "sha256");
        }

        /* =======================
         * HASHED PASSWORD FORMATS
         * =======================
         * 
         * Version 0:
         * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
         * (See also: SDL crypto guidelines v5.1, Part III)
         * Format: { 0x00, salt, subkey }
         */
        public static string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, Pbkdf2Count))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
            }

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashedPassword">hashedPassword must be of the format of HashWithPassword (salt + Hash(salt+input)</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
                throw new ArgumentNullException("hashedPassword");
            if (password == null)
                throw new ArgumentNullException("password");

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) password hash.
            if (hashedPasswordBytes.Length != (1 + SaltSize + Pbkdf2SubkeyLength) || hashedPasswordBytes[0] != (byte)0x00)
            {
                // Wrong length or version header.
                return false;
            }

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            var storedSubkey = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, Pbkdf2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Pbkdf2Count))
            {
                generatedSubkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
            }
            return ByteArraysEqual(storedSubkey, generatedSubkey);
        }

        private static string BinaryToHex(byte[] data)
        {
            var hex = new char[data.Length * 2];

            for (int iter = 0; iter < data.Length; iter++)
            {
                var hexChar = ((byte) (data[iter] >> 4));
                hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
                hexChar = ((byte)(data[iter] & 0xF));
                hex[iter * 2 + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
            }
            return new string(hex);
        }

        /// <summary>
        /// Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var areSame = true;
            for (int i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}