using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using FileExchanger.Services;
using System;

namespace FileExchanger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureService configureService = new ConfigureService();
            if (Config.Instance.IsFirstStart && !configureService.Configure(args))
            {
                Console.ReadLine();
                return;
            }
            if(Config.Instance.Security.Telegram.Enable)
                TelegramBotService.Instance.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
