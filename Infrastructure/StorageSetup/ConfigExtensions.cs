using System;
using System.Collections.Generic;
using System.Text;

namespace StorageSetup
{
    public static class ConfigExtensions
    {
        public static string TryGetEnvironmentVariableElseUseConfig(this string envVar)
        {
            var val = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrWhiteSpace(val))
            {
                val = Environment.GetEnvironmentVariable(envVar.Replace(":", "__"));
                if (string.IsNullOrWhiteSpace(val))
                {
                    return Config.Configuration[envVar];
                }
                return val;
            }
            return val;
        }
    }
}
