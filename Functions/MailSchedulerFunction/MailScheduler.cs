using System;
using Core;
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
            var dependencies = Dependencies.Setup(log);

            dependencies.DiagnosticLogging.Info($"CollectMailScheduler Timer trigger function executed at: {DateTime.Now}");

        }


    }

}
