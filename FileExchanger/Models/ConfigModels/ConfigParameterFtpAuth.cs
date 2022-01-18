using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterFtpAuth : ConfigParameterModel
    {
        public override string Parameter => "FTP_AUTH";

        protected override List<string> Templates => new List<string> { @"\S@\S" };
        public override string ErrorMessage => "Incorrect parameter 'FTP_AUTH'!\nFormat: user@password";
        public override string DefaultValue => "anon@anon";

        protected override string PathInConfigFile => "FTP:Auth";

        public override dynamic SaveChanage(dynamic config)
        {
            string[] tmp = Value.Split('@');
            config["FTP"]["Username"] = tmp[0];
            config["FTP"]["Password"] = tmp[1];
            return config;
        }
    }
}
