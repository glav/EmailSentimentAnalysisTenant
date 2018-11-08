using System;
using System.Threading.Tasks;
using Core;
using Core.Data;
using MailSanitiserFunction.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MailSanitiserFunction
{
    public static class MailSanitiser
    {
        [FunctionName("MailSanitiser")]
        public async static Task Run([QueueTrigger(DataStores.Queues.QueueNameCleanEmail)]string myQueueItem, ILogger log)
        {
            var dependencies = CoreDependencies.Setup(log);
            var receivedMessage = GenericActionMessage.FromString(myQueueItem);

            dependencies.DiagnosticLogging.Verbose($"MailSanitiser Timer trigger function executed at: {DateTime.UtcNow} UTC");

            var repo = new MailSanitiserRepository(dependencies);
            var engine = new MailSanitiserEngine(dependencies, repo);
            await engine.SanitiseMailAsync(receivedMessage);
        }
    }
}
