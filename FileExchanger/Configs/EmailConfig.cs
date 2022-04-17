namespace FileExchanger.Configs
{
    public class EmailConfig
    {
        public string Host => (string)Config.Instance.ConfigFile["Email"]["HOST"];
        public int Port => (int)Config.Instance.ConfigFile["Email"]["PORT"];
        public string Address => (string)Config.Instance.ConfigFile["Email"]["ADDRESS"];
        public string Password => (string)Config.Instance.ConfigFile["Email"]["PASSWORD"];
    }
}
