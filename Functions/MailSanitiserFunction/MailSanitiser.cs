using System;
using Core;
using Core.Data;
using MailSanitiserFunction.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MailSanitiserFunction
{
    public static class MailSanitiser
    {
        [FunctionName("MailSanitiser")]
        public static void Run([QueueTrigger(FunctionConstants.QueueNameCleanEmail)]string myQueueItem, ILogger log)
        {
            log.LogInformation("Starting MailSanitiser function");
            var dependencies = CoreDependencies.Setup(log);
            var receivedMessage = GenericActionMessage.FromString(myQueueItem);

            dependencies.DiagnosticLogging.Info($"MailSanitiser Timer trigger function executed at: {DateTime.UtcNow}");
        }
    }
}
