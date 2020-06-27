using System;
using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers
{
    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            using var generator = new RNGCryptoServiceProvider();
            var salt = new byte[16];
            generator.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        public static string GenerateHash(string salt, string password)
        {
            using var mySHA256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(salt + password);
            var ans = mySHA256.ComputeHash(bytes);

            return Convert.ToBase64String(ans);
        }

        public static bool ComparePasswords(string storedHash, string storedSalt, string enteredPassword)
        {
            var enteredHash = GenerateHash(storedSalt, enteredPassword);

            return storedHash == enteredHash;
        }
    }
}
