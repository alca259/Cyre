using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;

namespace Cyre.Api.Kernel
{
    public static class LogConfiguration
    {
        public static ILogger CreateSerilog(IConfiguration configuration, string nameSpace)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var template = "[{Timestamp:HH:mm:ss.fff} {Level:w3} {RequestContext}] [{UserContext}-{ClientContext}] {CommandContext} {TransactionContext} {SourceContext} {Message}{NewLine}{Exception}";
            var loggerConf = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.WithProperty("ApplicationContext", nameSpace)
                .Enrich.FromLogContext();

            if (envName == "Development")
                loggerConf.WriteTo.Console(outputTemplate: template);

            if (envName != "Development")
                loggerConf.WriteTo.File(configuration["Logging:Path"], outputTemplate: template);

            if (!string.IsNullOrWhiteSpace(configuration["ApplicationInsights:InstrumentationKey"]))
                loggerConf.WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces);

            return loggerConf
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
