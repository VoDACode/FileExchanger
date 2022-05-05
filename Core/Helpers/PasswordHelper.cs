using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers
{
    public static class PasswordHelper
    {
        public static string GetHash(string password) =>
            Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
