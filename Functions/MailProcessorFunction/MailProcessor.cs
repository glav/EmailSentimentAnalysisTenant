using System;
using System.Threading.Tasks;
using Core;
using Core.Data;
using MailProcessorFunction.Config;
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
        //public async static Task Run([HttpTrigger("GET")]string myQueueItem, ILogger log)
        {
            var dependencies = CoreDependencies.Setup(log);
            var receivedMessage = GenericActionMessage.FromString(myQueueItem);
            var analysisConfig = AnalysisConfiguration.PopulateConfigFromEnviromentVariables(dependencies);

            var now = DateTime.UtcNow;
            dependencies.DiagnosticLogging.Verbose("MailProcessor: Timer trigger function executed at: {now} UTC",now);

            var repo = new MailProcessorRepository(dependencies);
            var engine = new MailProcessingEngine(dependencies, repo, analysisConfig);
            await engine.AnalyseAllMailAsync(receivedMessage);

        }
    }
}
