using System;

namespace EmailSentimentAnalysis.Core
{
    public interface IDiagnosticLogger
    {
        void Verbose(string message);
        void Info(string message);
        void Debug(string message);
        void Error(string message);
        void Fatal(string message);

        void Verbose(string messageTemplate, params object[] args);
        void Info(string messageTemplate, params object[] args);
        void Debug(string messageTemplate, params object[] args);
        void Error(string messageTemplate, params object[] args);
        void Fatal(string messageTemplate, params object[] args);

        void Debug(Exception ex, string messageTemplate, params object[] args);
        void Error(Exception ex, string messageTemplate, params object[] args);
        void Fatal(Exception ex, string messageTemplate, params object[] args);


    }
}
