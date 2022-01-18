using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterDbPort : ConfigParameterModel
    {
        public override string Parameter => "DB_PORT";

        protected override List<string> Templates => new List<string> { @"[1-65535]\d" };

        protected override string PathInConfigFile => "Db:Port";
        public override dynamic SaveChanage(dynamic config)
        {
            config["Db"]["Port"] = int.Parse(Value);
            return config;
        }
    }
}
