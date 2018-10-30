using System;
using System.Threading.Tasks;
using CollectMailScheduler.Config;
using Core;
using MailSchedulerFunction.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailSchedulerFunction
{
    public static class MailSchedulerStatusListener
    {
        [FunctionName("MailSchedulerStatusListener")]
        public static async Task Listener([QueueTrigger(FunctionConfig.QueueNameTriggerEmail)]string queueItem, ILogger log)
        {
            log.LogInformation("Starting MailSchedulerStatusListener function");
            var coreDependencies = CoreDependencies.Setup(log);

            coreDependencies.DiagnosticLogging.Info($"MailSchedulerStatusListener received message at: {DateTime.UtcNow} UTC");

            var repo = new DataSchedulerRepository(coreDependencies);
            await repo.ClearMailOperationProgressAsync();

        }


    }

}
