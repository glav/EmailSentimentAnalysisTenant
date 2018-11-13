using System;
using System.Threading.Tasks;
using Core;
using Core.Data;
using MailProcessorFunction.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailProcessorFunction
{
    public static class MailProcessor
    {
        [FunctionName("MailProcessor")]
        public async static Task Run([QueueTrigger(DataStores.Queues.QueueNameProcessEmail)]string myQueueItem, ILogger log)
        {
            var dependencies = CoreDependencies.Setup(log);
            var receivedMessage = GenericActionMessage.FromString(myQueueItem);

            dependencies.DiagnosticLogging.Verbose($"MailProcessor: Timer trigger function executed at: {DateTime.UtcNow} UTC");

            var repo = new MailProcessorRepository(dependencies);
            var engine = new MailProcessingEngine(dependencies, repo);
            await engine.AnalyseAllMailAsync(receivedMessage);

        }
    }
}
