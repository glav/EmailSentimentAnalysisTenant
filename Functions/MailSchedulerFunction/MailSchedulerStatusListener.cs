using System;
using System.Threading.Tasks;
using Core;
using Core.Data;
using MailSchedulerFunction.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MailSchedulerFunction
{
    public static class MailSchedulerStatusListener
    {
        [FunctionName("MailSchedulerStatusListener")]
        public static async Task Listener([QueueTrigger(DataStores.Queues.QueueNameTriggerEmail)]string queueItem, ILogger log)
        {
            log.LogInformation("Starting MailSchedulerStatusListener function");
            var coreDependencies = CoreDependencies.Setup(log);

            coreDependencies.DiagnosticLogging.Info($"MailSchedulerStatusListener received message at: {DateTime.UtcNow} UTC");

            var repo = new DataSchedulerRepository(coreDependencies);
            await repo.ClearMailOperationProgressAsync();

        }


    }

}
