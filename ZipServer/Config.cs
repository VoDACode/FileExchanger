using Newtonsoft.Json;
using System;
using System.IO;
using ZipServer.Configs;

namespace ZipServer
{
    public class Config
    {
        private static Config instance;

        private DateTime lastUpdaat;

        private dynamic configFile;
        private string configText;
        private string dbConnect;
        public event Action OnUpdata;
        public string ConfigFileName => @"zipServerSettings.json";
        public Newtonsoft.Json.Linq.JObject ConfigFile => configFile;
        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Config();
                    Reload();
                    instance.lastUpdaat = File.GetLastWriteTime(instance.ConfigFileName);
                }
                else if (instance.lastUpdaat != File.GetLastWriteTime(instance.ConfigFileName))
                {
                    Reload();
                    instance.lastUpdaat = File.GetLastWriteTime(instance.ConfigFileName);
                }

                return instance;
            }
        }
        public static void Reload()
        {
            if (instance == null)
                instance = new Config();
            instance.configText = File.ReadAllText(instance.ConfigFileName);
            instance.configFile = JsonConvert.DeserializeObject(instance.configText);
            instance.dbConnect = $"Data Source={instance.ConfigFile["Db"]["Host"]},{instance.ConfigFile["Db"]["Port"]};" +
                $"Initial Catalog={instance.ConfigFile["Db"]["DbName"]};" +
                $"Persist Security Info=True;" +
                $"User ID={instance.ConfigFile["Db"]["UserId"]};" +
                $"Password={instance.ConfigFile["Db"]["Password"]}";
            instance.OnUpdata?.Invoke();
        }
        public static void Rewrite()
        {
            if (Instance == null)
                return;
            File.WriteAllText(instance.ConfigFileName, JsonConvert.SerializeObject(instance.ConfigFile));
            Instance.OnUpdata?.Invoke();
        }
        public string Token => (string)ConfigFile["Token"];
        public CorsConfig Cors { get; } = new CorsConfig();
        public string DbConnect => dbConnect;
        public FTPConfig Ftp { get; } = new FTPConfig();
    }
}
