using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailProcessorFunction
{
    class SentimentAnalysisLoggingAdapter : Glav.CognitiveServices.FluentApi.Core.Diagnostics.IDiagnosticLogger
    {
        private readonly CoreDependencyInstances _dependencies;
        public SentimentAnalysisLoggingAdapter(CoreDependencyInstances dependencies)
        {
            _dependencies = dependencies;
        }
        public void LogError(string message, string topic = null)
        {
            var logMsg = topic == null ? message : $"[{topic}] {message}";
            _dependencies.DiagnosticLogging.Error(logMsg);
        }

        public void LogError(Exception ex, string topic = null)
        {
            var logMsg = topic == null ? ex.GetBaseException().Message : $"[{topic}] {ex.GetBaseException().Message}";
            _dependencies.DiagnosticLogging.Error(ex,logMsg);
        }

        public void LogInfo(string message, string topic = null)
        {
            var logMsg = topic == null ? message : $"[{topic}] {message}";
            _dependencies.DiagnosticLogging.Info(logMsg);
        }

        public void LogWarning(string message, string topic = null)
        {
            var logMsg = topic == null ? message : $"[Warning-{topic}] {message}";
            _dependencies.DiagnosticLogging.Info(logMsg);
        }
    }
}
