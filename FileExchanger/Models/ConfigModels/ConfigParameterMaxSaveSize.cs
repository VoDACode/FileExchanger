using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterMaxSaveSize : ConfigParameterModel
    {
        public override string Parameter => "MaxSaveSize";
        protected override List<string> Templates => new List<string>()
        {
            @"\d Gb",
            @"\d Mb",
            @"\d Kb",
        };
        public override string ErrorMessage => "Incorrect parameter 'FILE_MaxSaveTime'!\nFormat: '1.5 Gb' or '512 Mb' or '900 Kb'";
        protected override string DefaultValue => "1.5 Gb";
        protected override string PathInConfigFile => "FileStorage:MaxSaveSize";

        public override object SaveChanage(dynamic config)
        {
            config["FileStorage"]["MaxSaveSize"] = Value;
            return config;
        }
    }
}
