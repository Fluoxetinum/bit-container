using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BitContainer.Service.Log
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                String baseAddress = "http://localhost:60836";
                if (args.Length > 0)
                    baseAddress = args[0];

                logger.Info($"Log service started on {baseAddress}");
                await CreateHostBuilder(baseAddress).RunConsoleAsync();
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Log service stopped because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(String baseAddress) =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.CaptureStartupErrors(false);
                    builder.UseUrls(baseAddress);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
