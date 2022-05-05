using FileExchanger.Helpers;

namespace FileExchanger.Configs
{
    public enum DefaultService { FileExchanger, FileStorage }
    public class ServicesConfig : Configurations
    {
        private FileExchangerConfig exchanger = new FileExchangerConfig();
        private FileStorageConfig storage = new FileStorageConfig();
        private ZipServerConfig zipServerConfig = new ZipServerConfig();
        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Services"];
        public FileStorageConfig FileStorage => storage;
        public FileExchangerConfig FileExchanger => exchanger;
        public ZipServerConfig ZipServer => zipServerConfig;
        public DefaultService DefaultService => ParseEnum<DefaultService>(ConfigSection["DefaultService"]);
    }
    public class FileExchangerConfig : Configurations
    {
        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Services"]["FileExchanger"];
        public bool Enable => ParseBool(ConfigSection["Enable"]);
        public bool UseAuth => ParseBool(ConfigSection["UseAuth"]);
        public double MaxSaveSize => SizeHelper.SizeParser((string)ConfigSection["MaxSaveSize"]);
        public double MaxSaveTime => (double)ConfigSection["MaxSaveTime"];
        public int MaxUploadCount => (int)ConfigSection["MaxUploadCount"];
    }
    public class FileStorageConfig : Configurations
    {
        public bool Enable => ParseBool(ConfigSection["Enable"]);
        public bool UseAuth => ParseBool(ConfigSection["UseAuth"]);
        public double MaxUploadSize => SizeHelper.SizeParser((string)ConfigSection["MaxUploadSize"]);
        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Services"]["FileStorage"];
    }
    public class ZipServerConfig : Configurations
    {
        public string Host => (string)ConfigSection["Host"];
        public int Port => (int)ConfigSection["Port"];
        public string Token => (string)ConfigSection["Token"];
        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Services"]["ZipServer"];
    }
}
