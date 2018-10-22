using System;
using Core;
using MailSchedulerFunction.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailSchedulerFunction
{
    public static class MailScheduler
    {
        [FunctionName("MailScheduler")]
        public static void Run([TimerTrigger("0/30 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Starting CollectMailScheduler function");
            var coreDependencies = Dependencies.Setup(log);

            coreDependencies.DiagnosticLogging.Info($"CollectMailScheduler Timer trigger function executed at: {DateTime.Now}");

            // Setup dependencies and invoke main processing component.
            var engine = new SchedulingEngine(coreDependencies, new DataSchedulerRepository());
            engine.ScheduleMailCollectionIfNotInProgress();

        }


    }

}
