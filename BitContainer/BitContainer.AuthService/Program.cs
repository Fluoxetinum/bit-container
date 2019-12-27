using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BitContainer.AuthService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                String baseAddress = "http://localhost:59884";
                if (args.Length > 0)
                    baseAddress = args[0];

                logger.Info($"Auth service started on {baseAddress}.");
                await CreateHostBuilder(baseAddress).RunConsoleAsync();
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Auth service stopped becaus of exception");
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
