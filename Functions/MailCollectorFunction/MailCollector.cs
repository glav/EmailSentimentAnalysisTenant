using System;
using System.Threading.Tasks;
using Core;
using MailCollectorFunction.Config;
using MailCollectorFunction.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailCollectorFunction
{
    public static class MailCollector
    {
        [FunctionName("MailCollector")]
        public static async Task Run([QueueTrigger(FunctionConstants.QueueNameCollectEmail)]string myQueueItem, ILogger log)
        {
            log.LogInformation("Starting MailCollector function");
            var dependencies = CoreDependencies.Setup(log);

            dependencies.DiagnosticLogging.Info($"MailCollector Queue trigger function executed at: {DateTime.UtcNow}");

            var mailConfig = EmailConfiguration.PopulateConfigFromEnviromentVariables(dependencies);
            var engine = new CollectionEngine(dependencies,new MailCollectionRepository(dependencies), mailConfig);
            await engine.PerformMailCollectionAsync();

        }
    }
}
