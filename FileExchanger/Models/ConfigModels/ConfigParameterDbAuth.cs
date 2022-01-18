using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterDbAuth : ConfigParameterModel
    {
        public override string Parameter => "DB_AUTH";

        protected override List<string> Templates => new List<string> { @"\S@\S" };
        public override string ErrorMessage => "Incorrect parameter 'DB_AUTH'!\nFormat: user@password";

        protected override string PathInConfigFile => "Db:Auth";

        public override dynamic SaveChanage(dynamic config)
        {
            string[] tmp = Value.Split('@');
            config["Db"]["UserId"] = tmp[0];
            config["Db"]["Password"] = tmp[1];
            return config;
        }
    }
}
