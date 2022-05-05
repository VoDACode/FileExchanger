using System.Collections.Generic;

namespace ZipServer.Configs
{
    public class CorsConfig
    {
        public string[] Cors { get
            {
                List<string> list = new List<string>();
                foreach (var item in Config.Instance.ConfigFile["Cors"])
                    list.Add((string)item);
                return list.ToArray();
            } 
        }
    }
}
