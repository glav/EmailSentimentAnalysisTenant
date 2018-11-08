using System;
using Core;
using Core.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailProcessorFunction
{
    public static class MailProcessor
    {
        [FunctionName("MailProcessor")]
        public static void Run([QueueTrigger(DataStores.Queues.QueueNameProcessEmail)]string myQueueItem, ILogger log)
        {
            var dependencies = CoreDependencies.Setup(log);

            dependencies.DiagnosticLogging.Verbose($"MailProcessor Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
