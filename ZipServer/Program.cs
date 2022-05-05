using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZipServer.Managers;

namespace ZipServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace((string)Config.Instance.Token))
            {
                Config.Instance.ConfigFile["Token"] = "".RandomString(256);
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://*:5051");
                });
    }
}
