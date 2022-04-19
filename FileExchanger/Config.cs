using FileExchanger.Models.UIModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using FileExchanger.Configs;
using System;

namespace FileExchanger
{
    public enum PermissionMode { Always, Never, Optionally }
    public class Config
    {
        private FTPConfig ftp = new FTPConfig();
        private AuthConfig auth = new AuthConfig();
        private EmailConfig email = new EmailConfig();
        private SecurityConfig security = new SecurityConfig();
        private ServicesConfig services = new ServicesConfig();

        private static Config instance;

        private DateTime lastUpdaat;

        private dynamic configFile;
        private string configText;
        private string dbConnect;
        private List<SavePatternModel> savePatterns = new List<SavePatternModel>();
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
                else if(instance.lastUpdaat != File.GetLastWriteTime(instance.ConfigFileName))
                {
                    Reload();
                    instance.lastUpdaat = File.GetLastWriteTime(instance.ConfigFileName);
                }
                
                return instance;
            }
        }
        public static void Reload()
        {
            if(instance == null)
                instance = new Config();
            instance.configText = File.ReadAllText(instance.ConfigFileName);
            instance.configFile = JsonConvert.DeserializeObject(instance.configText);
            instance.dbConnect = $"Data Source={instance.ConfigFile["Db"]["Host"]},{instance.ConfigFile["Db"]["Port"]};" +
                $"Initial Catalog={instance.ConfigFile["Db"]["DbName"]};" +
                $"Persist Security Info=True;" +
                $"User ID={instance.ConfigFile["Db"]["UserId"]};" +
                $"Password={instance.ConfigFile["Db"]["Password"]}";
            {
                foreach (var item in instance.ConfigFile["UI"]["SaveTimePatterns"])
                {
                    instance.savePatterns.Add(new SavePatternModel()
                    {
                        Value = (float)item["Value"],
                        Unit = (string)item["Unit"],
                    });
                }
            }
        }
        public static void Rewrite()
        {
            if (Instance == null)
                return;
            File.WriteAllText(Instance.ConfigFileName, JsonConvert.SerializeObject(Instance.ConfigFile));
        }
        public string ConfigFileName => @"appsettings.json";
        public Newtonsoft.Json.Linq.JObject ConfigFile => configFile;
        public string DbConnect => dbConnect;
        public FTPConfig Ftp => ftp;
        public AuthConfig Auth => auth;
        public EmailConfig Email => email;
        public SecurityConfig Security => security;
        public ServicesConfig Services => services;
        public bool IsFirstStart => (string)Instance.ConfigFile["FirstStart"] == "True";
        public List<SavePatternModel> SavePatterns => savePatterns;
        public override string ToString()
        {
            return configText;
        }
    }
}
