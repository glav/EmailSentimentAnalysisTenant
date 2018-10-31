using Core.Config;
using EmailSentimentAnalysis.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class CoreDependencies
    {
        public static CoreDependencyInstances Setup(Microsoft.Extensions.Logging.ILogger functionsLogger = null)
        {
            var diagLogger = DiagnosticLogger.CreateInstance(functionsLogger);

            return new CoreDependencyInstances
                (
                    diagLogger,
                    new EnvironmentValueReader(diagLogger)
                );
        }
    }

    public class CoreDependencyInstances
    {
        public CoreDependencyInstances(IDiagnosticLogger diagnosticLogging, IEnvironmentValueReader envValueReader)
        {
            DiagnosticLogging = diagnosticLogging;
            EnvironmentValueReader = envValueReader;
        }
        public IDiagnosticLogger DiagnosticLogging { get; private set; }
        public IEnvironmentValueReader EnvironmentValueReader { get; private set; }
    }
}
