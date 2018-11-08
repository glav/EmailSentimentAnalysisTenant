using System;
using Core;
using Core.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MailSanitiserFunction
{
    public static class MailSanitiser
    {
        [FunctionName("MailSanitiser")]
        public static void Run([QueueTrigger(DataStores.Queues.QueueNameCleanEmail)]string myQueueItem, ILogger log)
        {
            var dependencies = CoreDependencies.Setup(log);
            var receivedMessage = GenericActionMessage.FromString(myQueueItem);

            dependencies.DiagnosticLogging.Verbose($"MailSanitiser Timer trigger function executed at: {DateTime.UtcNow}");
        }
    }
}
