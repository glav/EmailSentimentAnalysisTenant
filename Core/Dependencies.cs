﻿using Core.Config;
using EmailSentimentAnalysis.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class Dependencies
    {
        public static DependencyInstances Setup(Microsoft.Extensions.Logging.ILogger functionsLogger = null)
        {
            var config = new AppConfig();
            var diagLogger = DiagnosticLogger.CreateInstance(config, functionsLogger);

            return new DependencyInstances
                (
                    config,
                    diagLogger,
                    new EnvironmentValueReader(diagLogger)
                );
        }
    }

    public class DependencyInstances
    {
        public DependencyInstances(IAppConfig appConfiguration, IDiagnosticLogger diagnosticLogging, IEnvironmentValueReader envValueReader)
        {
            AppConfiguration = appConfiguration;
            DiagnosticLogging = diagnosticLogging;
            EnvironmentValueReader = envValueReader;
        }
        public IAppConfig AppConfiguration { get; private set; }
        public IDiagnosticLogger DiagnosticLogging { get; private set; }
        public IEnvironmentValueReader EnvironmentValueReader { get; private set; }
    }
}
