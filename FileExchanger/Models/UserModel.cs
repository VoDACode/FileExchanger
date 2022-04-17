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
        public double MaxFileSize { get; set; } = Config.Instance.Services.FileExchanger.MaxSaveSize;
        public int MaxFileCount { get; set; } = Config.Instance.Services.FileExchanger.MaxUploadCount;
        public double MaxSaveFileTime { get; set; } = Config.Instance.Services.FileExchanger.MaxSaveTime;
    }
}
