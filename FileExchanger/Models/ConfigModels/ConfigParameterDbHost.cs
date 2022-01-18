using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterDbHost : ConfigParameterModel
    {
        public override string Parameter => "DB_HOST";

        protected override List<string> Templates => new List<string>
        {
            @"\d{3}.\d{3}.\d{3}.\d{3}",
            @"\S"
        };

        protected override string PathInConfigFile => "Db:Host";

        public override object SaveChanage(dynamic config)
        {
            config["Db"]["Host"] = Value;
            return config;
        }
    }
}
