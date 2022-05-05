namespace FileExchanger.Configs
{
    public class FTPConfig
    {
        public string Username => (string)Config.Instance.ConfigFile["FTP"]["Username"];
        public string Password => (string)Config.Instance.ConfigFile["FTP"]["Password"];
        public int Port => (int)Config.Instance.ConfigFile["FTP"]["Port"];
        public string Host => (string)Config.Instance.ConfigFile["FTP"]["Host"];
        public string AuthPath => $"{(bool.Parse((string)Config.Instance.ConfigFile["FTP"]["EnableSFTP"]) ? "s" : "")}ftp://{Username}:{Password}@{Host}:{Port}/{(string)Config.Instance.ConfigFile["FTP"]["RootPath"]}";
        public string Path => $"{(bool.Parse((string)Config.Instance.ConfigFile["FTP"]["EnableSFTP"]) ? "s" : "")}ftp://{Host}:{Port}/{(string)Config.Instance.ConfigFile["FTP"]["RootPath"]}";
    }
}
