using Cyre.Api.Kernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace Cyre.Api
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;

        public static int Main(string[] args)
        {
            Console.Title = "Api Cyre";

            var configuration = AppConfiguration.GetConfiguration<Program>();
            Log.Logger = LogConfiguration.CreateSerilog(configuration, Namespace);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", Namespace);

                var host = BuildWebHost(configuration, args);

                Log.Information("Starting web host ({ApplicationContext})...", Namespace);

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Namespace);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        private static IHost BuildWebHost(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webhost =>
                {
                    webhost.CaptureStartupErrors(false);
                    webhost.UseStartup<Startup>();
                    webhost.UseConfiguration(configuration);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging((hostintContext, logging) => logging.ClearProviders())
                .UseSerilog()
                .Build();
    }
}
