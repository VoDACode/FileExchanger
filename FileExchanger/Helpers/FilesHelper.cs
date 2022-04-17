using FileExchanger.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FileExchanger.Helpers
{
    public static class FilesHelper
    {
        public static string GeneranionKey(DirectoryModel directory, DateTime createTime)
        {
            return Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{directory.Key}\0{directory.Id}\0{createTime.Ticks}"))).Replace("/", "").Replace("=", "");
        }
    }
}
