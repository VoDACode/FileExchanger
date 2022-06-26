using System.Linq;

namespace System
{
    public static class StringHelper
    {
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmopqrstuvwxyz0123456789";
        public static string RandomString(this string str, int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(_chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
