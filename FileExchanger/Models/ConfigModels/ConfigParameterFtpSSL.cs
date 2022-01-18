using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterFtpSSL : ConfigParameterModel
    {
        public override string Parameter => "FTP_SSL";
        protected override List<string> Templates => new List<string> { @"[0-1]\d" };
        public override string ErrorMessage => "Incorrect parameter 'FTP_SSL'!\nFormat: 1 or 0";
        public override string DefaultValue => "0";
        protected override string PathInConfigFile => "FTP:EnableSFTP";
        public override dynamic SaveChanage(dynamic config)
        {
            config["FTP"]["EnableSFTP"] = Value == "0" ? "False" : "True";
            return config;
        }
    }
}
