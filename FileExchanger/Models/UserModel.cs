using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string UserAgent { get; set; }
        public string Host { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime RegistrationDate { get; set; }
        public double MaxFileSize { get; set; } = Math.Pow(1024, 3) * 1.5;
        public int MaxFileCount { get; set; } = 2;
        public double MaxSaveFileTime { get; set; } = TimeSpan.FromDays(2).TotalSeconds;
    }
}
