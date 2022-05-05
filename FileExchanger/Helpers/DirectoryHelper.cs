using System;
using System.Security.Cryptography;
using System.Text;
using Core.Models;
using FileExchanger.Models;

namespace FileExchanger.Helpers
{
    public static class DirectoryHelper
    {
        public static string GeneranionKey(DirectoryModel root, DateTime createTime)
        {
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{root.Key}\0{root.Id}\0{createTime.Ticks}"))).Replace("/", "").Replace("=", "");
        }
    }
}
