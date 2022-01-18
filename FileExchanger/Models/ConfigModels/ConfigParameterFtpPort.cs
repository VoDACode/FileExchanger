using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterFtpPort : ConfigParameterModel
    {
        public override string Parameter => "FTP_PORT";

        protected override List<string> Templates => new List<string> { @"[1-65535]\d" };
        protected override string DefaultValue => "21";

        protected override string PathInConfigFile => "FTP:Port";
        public override dynamic SaveChanage(dynamic config)
        {
            config["FTP"]["Port"] = int.Parse(Value);
            return config;
        }
    }
}
