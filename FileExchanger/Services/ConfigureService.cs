using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileExchanger.Models.ConfigModels;
using Newtonsoft.Json;

namespace FileExchanger.Services
{
    class ConfigureService
    {
        private List<ConfigParameterModel> ConfigParameters { get; set; } = new List<ConfigParameterModel>();
        
        public ConfigureService()
        {
            ConfigParameters.Add(new ConfigParameterDbHost());
            ConfigParameters.Add(new ConfigParameterDbPort());
            ConfigParameters.Add(new ConfigParameterDbName());
            ConfigParameters.Add(new ConfigParameterDbAuth());
            ConfigParameters.Add(new ConfigParameterFtpHost());
            ConfigParameters.Add(new ConfigParameterFtpPort());
            ConfigParameters.Add(new ConfigParameterFtpAuth());
            ConfigParameters.Add(new ConfigParameterFtpSSL());
            ConfigParameters.Add(new ConfigParameterMaxSaveSize());
            ConfigParameters.Add(new ConfigParameterMaxSaveTime());
            ConfigParameters.Add(new ConfigParameterMaxUploadCount());
        }

        public bool Configure(string[] args)
        {
            if(!parseArguments(args))
                return false;

            var editedParameters = ConfigParameters.Where(p => p.IsSet).ToList();

            if(Config.IsFirstStart && editedParameters.Count < ConfigParameters.Count(p => p.IsRequired))
            {
                Console.WriteLine("Pleace configure project!\n\t--help\tFor help");
                return false;
            }

            if(Config.IsFirstStart && !ConfigParameters.Any(p => p.Parameter == "DB_NAME" && p.IsSet))
            {
                ConfigParameters.Single(p => p.Parameter == "DB_NAME").Value = $"FileExchanger_{DateTime.Now.Ticks}";
            }

            saveChanges();
            
            return true;
        }

        private bool parseArguments(string[] args)
        {
            if (args.Length == 0 || args.Any(p => p == "--help") || !ConfigParameters.Any(p => p.Check(args[0])))
            {
                help();
                return false;
            }
            string param = "";
            string val = "";
            for (int i = 0; i <= args.Length; i++)
            {
                if (i >= args.Length || (ConfigParameters.Any(p => p.Check(args[i])) && param != ""))
                {
                    var conf = ConfigParameters.SingleOrDefault(p => p.Check(param));
                    if (!conf.IsValid(val))
                    {
                        Console.WriteLine($"ERROR SYNTAX: ...{param} {val}...\nMessage:{conf.ErrorMessage}\n\n Use --help to help\n");
                        return false;
                    }
                    conf.Value = val;
                    val = "";
                    param = "";
                }
                if (i < args.Length)
                {
                    if (param == "")
                        param = args[i];
                    else
                        val += (val == "" ? "" : " ") + args[i];
                }
            }
            return true;
        }

        private void saveChanges()
        {
            var config = Config.ConfigFile;
            foreach(var conf in ConfigParameters.Where(p => p.IsSet))
                conf.SaveChanage(config);
            if (Config.IsFirstStart)
                config["FirstStart"] = "False";
            File.WriteAllText(Config.ConfigFileName, JsonConvert.SerializeObject(config));
        }

        private void help()
        {
            foreach (var item in ConfigParameters)
            {
                var str = $"\t-{item.Parameter}\n";
                if (!item.IsRequired)
                    str += $"\tDefault: '{item.DefaultValue}'";
                Console.WriteLine($"{str}\n");
            }
            Console.WriteLine($"\t--help\n\tPrint this menu :D\n\n");
        }
    }
}
