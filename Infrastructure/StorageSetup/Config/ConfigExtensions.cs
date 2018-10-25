using Core;
using Serilog;
using System;

namespace StorageSetup.Config
{
    public static class ConfigExtensions
    {
        private static DependencyInstances _dependencyInstances;
        static ConfigExtensions()
        {
            _dependencyInstances = Dependencies.Setup();
        }
        public static string TryGetEnvironmentVariableElseUseConfig(this string[] environmentVariables)
        {
            var val = _dependencyInstances.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(environmentVariables);
            if (!string.IsNullOrWhiteSpace(val))
            {
                return val;
            }

            foreach (var ev in environmentVariables)
            {
                _dependencyInstances.DiagnosticLogging.Info("Getting Connection string setting from config file for: {0}", ev);
                val = ConfigReader.Configuration[ev];
                if (!string.IsNullOrWhiteSpace(val))
                {
                    _dependencyInstances.DiagnosticLogging.Info("Found setting in config for key: {0}",ev);
                    return val;
                }
            }
            return null;
        }

    }
}
