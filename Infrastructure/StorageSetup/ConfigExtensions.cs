using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageSetup
{
    public static class ConfigExtensions
    {
        public static string TryGetEnvironmentVariableElseUseConfig(this string envVar)
        {
            Log.Logger.Information("Getting Connection string environment setting for: {0}", envVar);
            var val = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrWhiteSpace(val))
            {
                var envVarReplaced = envVar.Replace(":", "__");
                Log.Logger.Information("Getting Connection string environment setting for: {0}", envVarReplaced);
                val = Environment.GetEnvironmentVariable(envVarReplaced);
                if (string.IsNullOrWhiteSpace(val))
                {
                    Log.Logger.Information("Nothing found, Getting Connection string from config file");
                    return Config.Configuration[envVar];
                }
                return val;
            }
            return val;
        }
    }
}
