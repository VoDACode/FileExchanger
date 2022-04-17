using System;
namespace FileExchanger.Configs
{
    public class SecurityConfig : Configurations
    {
        private AccountsConfig accounts = new AccountsConfig();
        private TelegramConfig telegram = new TelegramConfig();
        public PermissionMode Authorization
        {
            get
            {
                if (Config.Instance.Services.FileStorage.UseAuth && Config.Instance.Services.FileExchanger.UseAuth)
                    return PermissionMode.Always;
                if((Config.Instance.Services.FileStorage.UseAuth && !Config.Instance.Services.FileExchanger.UseAuth) ||
                    (!Config.Instance.Services.FileStorage.UseAuth && Config.Instance.Services.FileExchanger.UseAuth))
                    return PermissionMode.Optionally;
                return PermissionMode.Never;
            }
        }
        public AccountsConfig Accounts => accounts;
        public TelegramConfig Telegram => telegram;

        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Security"];
    }

    public class AccountsConfig : Configurations
    {
        public bool Enable => ParseBool(ConfigSection["Enable"]);
        public bool EnableRegistration => ParseBool(ConfigSection["EnableRegistration"]);

        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Security"]["Accounts"];
    }

    public class TelegramConfig : Configurations
    {
        public bool Enable => ParseBool(ConfigSection["Enable"]);
        public string Token => (string)ConfigSection["Token"];

        protected override dynamic ConfigSection => Config.Instance.ConfigFile["Security"]["Telegram"];
    }
}
