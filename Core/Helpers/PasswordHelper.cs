using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers
{
    public static class PasswordHelper
    {
        public static byte[] GetSecureSalt
        {
            get
            {
                byte[] salt = new byte[64];
                Random r = new Random();
                for (int i = 0; i < salt.Length; i++)
                    salt[i] = (byte)r.Next(0, 255);
                return salt.ToArray();
            }
        }
        public static string GetHash(string password, byte[] salt)
        {
            byte[] derivedKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount: 100000, 64);
            return Convert.ToBase64String(derivedKey);
        }
    }
}
