using System.Collections.Generic;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterMaxUploadCount : ConfigParameterModel
    {
        public override string Parameter => "MaxUploadCount";

        protected override List<string> Templates => new List<string> { @"[1-999999]\d" };
        protected override string DefaultValue => "2";

        protected override string PathInConfigFile => "FileStorage:MaxUploadCount";

        public override object SaveChanage(dynamic config)
        {
            config["FileStorage"]["MaxUploadCount"] = int.Parse(Value);
            return config;
        }
    }
}
