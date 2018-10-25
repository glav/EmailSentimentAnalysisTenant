using Serilog;
using System;

namespace StorageSetup.Config
{
    public static class ConfigExtensions
    {
        public static string TryGetEnvironmentVariableElseUseConfig(this string[] environmentVariables)
        {
            Log.Logger.Information("Try getting environment settings for: {0}", environmentVariables);
            if (environmentVariables == null || environmentVariables.Length == 0)
            {
                return null;
            }

            var val = environmentVariables.GetEnvironmentValueThatIsNotEmpty();
            if (!string.IsNullOrWhiteSpace(val))
            {
                return val;
            }

            foreach (var ev in environmentVariables)
            {
                Log.Logger.Information("Getting Connection string setting from config file for: {0}", ev);
                val = ConfigReader.Configuration[ev];
                if (!string.IsNullOrWhiteSpace(val))
                {
                    Log.Logger.Information("Found setting in config for key: {0}",ev);
                    return val;
                }
            }
            return null;
        }

        public static string GetEnvironmentValueThatIsNotEmpty(this string[] environmentVariables)
        {
            if (environmentVariables == null || environmentVariables.Length == 0)
            {
                return null;
            }
            foreach (var ev in environmentVariables)
            {
                Log.Logger.Information("Getting environment setting for: {0}", ev);
                var val = Environment.GetEnvironmentVariable(ev);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    Log.Logger.Information("Found environment setting for: {0}", ev);
                    return val;
                }
                Log.Logger.Information("Environment setting for: {0} not present", ev);
            }
            Log.Logger.Information("No Environment settings found for: {0}", environmentVariables);
            return null;
        }
    }
}
