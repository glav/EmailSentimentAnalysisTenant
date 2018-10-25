using System;
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
        public static void Listener([QueueTrigger(FunctionConfig.QueueNameProcessEmail)]string queueItem, ILogger log)
        {
            log.LogInformation("Starting MailSchedulerStatusListener function");
            var coreDependencies = CoreDependencies.Setup(log);

            coreDependencies.DiagnosticLogging.Info($"MailSchedulerStatusListener received message at: {DateTime.UtcNow} UTC");

            new DataSchedulerRepository(coreDependencies).ClearMailOperationProgress();

        }


    }

}
