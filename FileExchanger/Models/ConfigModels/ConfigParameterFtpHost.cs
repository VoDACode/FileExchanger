using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterFtpHost : ConfigParameterModel
    {
        public override string Parameter => "FTP_HOST";

        protected override List<string> Templates => new List<string> 
        { 
            @"\d{3}.\d{3}.\d{3}.\d{3}",
            @"\S"
        };

        protected override string PathInConfigFile => "FTP:Host";

        public override object SaveChanage(dynamic config)
        {
            config["FTP"]["Host"] = Value;
            return config;
        }
    }
}
