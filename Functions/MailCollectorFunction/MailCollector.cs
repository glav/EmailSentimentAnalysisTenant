using System;
using System.Threading.Tasks;
using Core;
using MailCollectorFunction.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailCollectorFunction
{
    public static class MailCollector
    {
        [FunctionName("MailCollector")]
        public static async Task Run([QueueTrigger(FunctionConfig.QueueNameCollectEmail)]string myQueueItem, ILogger log)
        {
            log.LogInformation("Starting MailCollector function");
            var dependencies = CoreDependencies.Setup(log);

            dependencies.DiagnosticLogging.Info($"MailCollector Queue trigger function executed at: {DateTime.UtcNow}");

        }
    }
}
