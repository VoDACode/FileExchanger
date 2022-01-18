using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileExchanger.Models.ConfigModels
{
    class ConfigParameterMaxSaveTime : ConfigParameterModel
    {
        public override string Parameter => "MaxSaveTime";
        protected override List<string> Templates => new List<string>
        {
            @"\dd",
            @"\dh",
            @"\dm",
            @"\ds",
        };
        public override string ErrorMessage => "Incorrect parameter 'FILE_MaxSaveTime'!\nFormat: '1d 5h 40m 50s'";
        public override string DefaultValue => "1d";

        protected override string PathInConfigFile => "FileStorage:MaxSaveTime";

        public override bool IsValid(string val)
        {
            Dictionary<string, bool> list = new Dictionary<string, bool>();
            for (int i = 0; i < Templates.Count; i++)
                list.Add(Templates[i], false);
            var tmpStr = val.Split(' ');


            for (int i = 0; i < tmpStr.Length; i++)
            {
                bool res = false;
                foreach (var item in list.Where(p => !p.Value))
                {
                    res = new Regex(item.Key).IsMatch(tmpStr[i]);
                    if (res)
                    {
                        list[item.Key] = true;
                        break;
                    }
                }
                if (!res)
                    return false;
            }

            return true;
        }

        public override dynamic SaveChanage(dynamic config)
        {
            config["FileStorage"]["MaxSaveTime"] = getNum('s') + getNum('m') * 60 + getNum('h') * 3600 + getNum('d') * 86400;

            int getNum(char param)
            {
                int tIndex = Value.IndexOf(param);
                if (tIndex == -1)
                    return 0;
                int tDataIndex = 0;
                string tData = "";
                for (int i = tIndex; i >= 0; i--)
                {
                    if (Value[i] == ' ')
                        break;
                    tDataIndex = i;
                }
                for (int i = tDataIndex; i < tIndex; i++)
                    tData += Value[i];
                return int.Parse(tData);
            }
            return config;
        }
    }
}
