using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileExchanger.Models.ConfigModels
{
    abstract class ConfigParameterModel
    {
        private string value;
        public abstract string Parameter { get; }
        protected abstract string PathInConfigFile { get; }
        public virtual string DefaultValue => null;
        public string Value { get => value; set
            {
                IsSet = true;
                this.value = value;
            }

        }
        protected abstract List<string> Templates { get; }
        public virtual string ErrorMessage => $"Incorrect parameter '{Parameter}'!";
        public bool IsRequired => DefaultValue == null;
        public bool IsSet { get; private set; }
        public bool IsError { get; protected set; }

        public bool Check(string parametr) => (Parameter.First() == '-' ? Parameter : $"-{Parameter}") == parametr;

        public virtual bool IsValid(string val)
        {
            for(int i = 0; i < Templates.Count; i++)
                if(new Regex(Templates[i]).IsMatch(val))
                    return true;
            return false;
        }

        public void SaveChanage()
        {
            dynamic config = Config.Instance.ConfigFile;
            config = SaveChanage(config);
            File.WriteAllText(Config.Instance.ConfigFileName, JsonConvert.SerializeObject(config));
        }

        public abstract object SaveChanage(dynamic config);
    }
}
