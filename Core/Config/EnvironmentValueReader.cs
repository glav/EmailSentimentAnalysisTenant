using EmailSentimentAnalysis.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Config
{
    public class EnvironmentValueReader : IEnvironmentValueReader
    {
        private readonly IDiagnosticLogger _logger;

        public EnvironmentValueReader(IDiagnosticLogger logger)
        {
            _logger = logger;
        }

        public string GetEnvironmentValueThatIsNotEmpty(string[] environmentVariables, string defaultValue = null)
        {
            if (environmentVariables == null || environmentVariables.Length == 0)
            {
                return null;
            }
            foreach (var ev in environmentVariables)
            {
                _logger.Info("Getting environment setting for: {0}", ev);
                var val = Environment.GetEnvironmentVariable(ev);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    _logger.Info("Found environment setting for: {0}", ev);
                    return val;
                }
                _logger.Info("Environment setting for: {0} not present", ev);
            }
            _logger.Info("No Environment settings found for: {0}", environmentVariables);
            return defaultValue;
        }

    }
}
