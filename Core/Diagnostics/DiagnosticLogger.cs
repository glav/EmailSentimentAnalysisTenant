using System;
using Core.Config;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ILogger;

namespace EmailSentimentAnalysis.Core
{
    public class DiagnosticLogger : IDiagnosticLogger
    {
        private ILogger _logger;
        private readonly IAppConfig _appConfig;
        private readonly Microsoft.Extensions.Logging.ILogger _functionsLogger;


        protected DiagnosticLogger(IAppConfig appConfig, Microsoft.Extensions.Logging.ILogger functionsLogger)
        {
            _appConfig = appConfig;
            _functionsLogger = functionsLogger;
        }

        private void Initialise()
        {
            if (!_appConfig.IsHostedInAzure)
            {
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
            {
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.ILogger(_functionsLogger)
                    .CreateLogger();
            }

            Log.Logger = _logger;
        }

        public static DiagnosticLogger CreateInstance(IAppConfig appConfig, Microsoft.Extensions.Logging.ILogger functionsLogger)
        {
            var diagLogger = new DiagnosticLogger(appConfig, functionsLogger);
            diagLogger.Initialise();
            return diagLogger;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string messageTemplate, params object[] args)
        {
            _logger.Debug(messageTemplate, args);
        }

        public void Debug(Exception ex, string messageTemplate, params object[] args)
        {
            _logger.Debug(ex, messageTemplate, args);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string messageTemplate, params object[] args)
        {
            _logger.Error(messageTemplate, args);
        }

        public void Error(Exception ex, string messageTemplate, params object[] args)
        {
            _logger.Error(ex, messageTemplate, args);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string messageTemplate, params object[] args)
        {
            _logger.Fatal(messageTemplate, args);
        }

        public void Fatal(Exception ex, string messageTemplate, params object[] args)
        {
            _logger.Fatal(ex, messageTemplate, args);
        }

        public void Info(string message)
        {
            _logger.Information(message);
        }

        public void Info(string messageTemplate, params object[] args)
        {
            _logger.Information(messageTemplate, args);
        }

        public void Verbose(string message)
        {
            _logger.Verbose(message);
        }

        public void Verbose(string messageTemplate, params object[] args)
        {
            _logger.Verbose(messageTemplate, args);
        }
    }
}