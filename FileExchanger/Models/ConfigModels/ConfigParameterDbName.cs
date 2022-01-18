using System;
using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterDbName : ConfigParameterModel
    {
        public override string Parameter => "DB_NAME";
        protected override List<string> Templates => new List<string> { @"\S" };
        public override string ErrorMessage => "Please select DB!";
        protected override string DefaultValue => $"FileExchanger_{DateTime.Now.Ticks}";

        protected override string PathInConfigFile => "Db:DbName";

        public override object SaveChanage(dynamic config)
        {
            config["Db"]["DbName"] = Value;
            return config;
        }
    }
}
