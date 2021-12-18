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
        public static dynamic ConfigFile => JsonConvert.DeserializeObject(File.ReadAllText(@".\appsettings.json"));
        #region FTP
        public static string FtpUsername => (string)ConfigFile["FTP"]["Username"];
        public static string FtpPassword => (string)ConfigFile["FTP"]["Password"];
        public static int FtpPort => (int)ConfigFile["FTP"]["Port"];
        public static string FtpHost => (string)ConfigFile["FTP"]["Host"];
        public static string FtpPath => $"{(bool.Parse((string)ConfigFile["FTP"]["EnableSFTP"]) ? "s" : "")}ftp://{FtpHost}:{FtpPort}/{(string)ConfigFile["FTP"]["RootPath"]}";
        #endregion
    }
}
