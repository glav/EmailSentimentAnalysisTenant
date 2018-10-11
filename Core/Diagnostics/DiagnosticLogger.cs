using Serilog;
using Serilog.Events;

namespace EmailSentimentAnalysis.Core
{
    public class DiagnosticLogger
    {
        private ILogger _logger;

        public void Initialise()
        {
            _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            Log.Logger = _logger;
        }
    }
}