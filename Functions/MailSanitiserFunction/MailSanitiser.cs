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

            var now = DateTime.UtcNow;
            dependencies.DiagnosticLogging.Verbose("Sanitisation: MailSanitiser Timer trigger function executed at: {now} UTC",now);

            var repo = new MailSanitiserRepository(dependencies);
            var statusRepo = new StatusRepository(dependencies);
            var engine = new MailSanitiserEngine(dependencies, repo,statusRepo);
            await engine.SanitiseMailAsync(receivedMessage);
        }
    }
}
