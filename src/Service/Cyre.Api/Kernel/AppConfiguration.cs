using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Cyre.Api.Kernel
{
    public static class AppConfiguration
    {
        public static IConfiguration GetConfiguration<T>() where T : class
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var buildConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (envName == "Development")
            {
                buildConfig = buildConfig.AddUserSecrets<T>();
            }

            return buildConfig.Build();
        }
    }
}
