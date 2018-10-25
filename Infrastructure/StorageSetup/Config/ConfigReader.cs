using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageSetup.Config
{
    public static class ConfigReader
    {
        public static IConfigurationRoot Configuration { get; set; }

        static ConfigReader()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            if (Environment.GetEnvironmentVariable("BuildConfiguration") == "development")
            {
                builder.AddUserSecrets<StorageSetup.Program>();
            }

            Configuration = builder.Build();
        }

        public static string ConnectionStringStorageAccount
            => new string[]{
                "ConnectionStrings:StorageAccount",
                "ConnectionStrings__StorageAccount",
                "TF_STORAGE_CONNECTION_STRING"
            }.TryGetEnvironmentVariableElseUseConfig();
    }
}
