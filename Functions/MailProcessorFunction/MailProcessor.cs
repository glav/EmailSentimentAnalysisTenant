using System;
using Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailProcessorFunction
{
    public static class MailProcessor
    {
        [FunctionName("MailProcessor")]
        public static void Run([QueueTrigger("myqueue-items", Connection = "")]string myQueueItem, ILogger log)
        {
            log.LogInformation("Starting MailProcessor function");
            var dependencies = CoreDependencies.Setup(log);

            dependencies.DiagnosticLogging.Info($"MailProcessor Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
