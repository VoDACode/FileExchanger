using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using FileExchanger.Services;

namespace FileExchanger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureService configureService = new ConfigureService();
            if (!configureService.Configure(args))
                return;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseIISIntegration();
                });
    }
}
