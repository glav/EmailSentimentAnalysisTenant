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
                _logger.Verbose("Getting environment setting for: {ev}",ev);
                var val = Environment.GetEnvironmentVariable(ev);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    _logger.Info("Found environment setting: {ev}",ev);
                    return val;
                }
                _logger.Info($"Environment setting for: {ev} not present");
            }
            _logger.Info("No Environment settings found for: {environmentVariables}", environmentVariables);
            return defaultValue;
        }

    }
}
