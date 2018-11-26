using System;
using System.Threading.Tasks;
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
        public static async Task Run([TimerTrigger("0/15 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            var coreDependencies = CoreDependencies.Setup(log);

            var now = DateTime.UtcNow;
            coreDependencies.DiagnosticLogging.Verbose("MailScheduler: Timer trigger function executed at: {now} UTC",now);

            // Setup dependencies and invoke main processing component.
            var engine = new SchedulingEngine(coreDependencies, new DataSchedulerRepository(coreDependencies));
            await engine.ScheduleMailCollectionIfNotInProgressAsync();

        }
    }

}
