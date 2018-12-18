using System;
using System.Threading.Tasks;
using Core;
using Core.Data;
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
        public static async Task Run([QueueTrigger(DataStores.Queues.QueueNameCollectEmail)]string myQueueItem, ILogger log)
        {
            var dependencies = CoreDependencies.Setup(log);
            var statusRepo = new StatusRepository(dependencies);
            var mailRepo = new MailCollectionRepository(dependencies);

            var now = DateTime.UtcNow;
            dependencies.DiagnosticLogging.Verbose("MailCollection: MailCollector Queue trigger function executed at: {now} UTC", now);

            var mailConfig = EmailConfiguration.PopulateConfigFromEnviromentVariables(dependencies);
            var engine = new CollectionEngine(dependencies,mailRepo,statusRepo, mailConfig);

            var queueMsg = GenericActionMessage.FromString(myQueueItem);
            await engine.PerformMailCollectionAsync(queueMsg);

        }
    }
}
