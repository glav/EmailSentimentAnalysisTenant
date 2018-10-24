using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageSetup
{
    public static class Config
    {
        public static IConfigurationRoot Configuration { get; set; }

        static Config()
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

        public static string ConnectionStringStorageAccount => "ConnectionStrings:StorageAccount".TryGetEnvironmentVariableElseUseConfig();
        public static string TFConnectionString => "TF_STORAGE_CONNECTION_STRING".TryGetEnvironmentVariableElseUseConfig();
    }
}
