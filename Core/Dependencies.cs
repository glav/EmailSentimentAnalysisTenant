using Core.Config;
using EmailSentimentAnalysis.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class Dependencies
    {
        public static DependencyInstances Setup(Microsoft.Extensions.Logging.ILogger functionsLogger)
        {
            var config = new AppConfig();
            return new DependencyInstances
                (
                    config,
                    new DiagnosticLogger(config, functionsLogger)
                );
        }
    }

    public class DependencyInstances
    {
        public DependencyInstances(IAppConfig appConfiguration, IDiagnosticLogger diagnosticLogging)
        {
            AppConfiguration = AppConfiguration;
            DiagnosticLogging = diagnosticLogging;
        }
        public IAppConfig AppConfiguration { get; private set; }
        public IDiagnosticLogger DiagnosticLogging { get; private set; }
    }
}
