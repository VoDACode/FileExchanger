﻿using FileExchanger.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger
{
    public static class Config
    {
        public static dynamic ConfigFile => JsonConvert.DeserializeObject(File.ReadAllText(@"appsettings.json"));
        #region FTP
        public static string FtpUsername => (string)ConfigFile["FTP"]["Username"];
        public static string FtpPassword => (string)ConfigFile["FTP"]["Password"];
        public static int FtpPort => (int)ConfigFile["FTP"]["Port"];
        public static string FtpHost => (string)ConfigFile["FTP"]["Host"];
        public static string FtpPath => $"{(bool.Parse((string)ConfigFile["FTP"]["EnableSFTP"]) ? "s" : "")}ftp://{FtpHost}:{FtpPort}/{(string)ConfigFile["FTP"]["RootPath"]}";
        #endregion

        #region FileStorage
        public static double MaxSaveSize => SizeHelper.SizeParser((string)ConfigFile["FileStorage"]["MaxSaveSize"]);
        public static double MaxSaveTime => (double)ConfigFile["FileStorage"]["MaxSaveTime"];
        public static int MaxUploadCount => (int)ConfigFile["FileStorage"]["MaxUploadCount"];
        #endregion
    }
}
