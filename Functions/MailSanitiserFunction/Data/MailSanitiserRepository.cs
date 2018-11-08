using Core;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailSanitiserFunction.Data
{
    class MailSanitiserRepository : BaseCloudStorageRepository, IMailSanitiserRepository
    {
        public MailSanitiserRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {
        }

        public async Task LodgeMailSanitisedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            Dependencies.DiagnosticLogging.Info("Lodging Mail Sanitised Acknowledgement");
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(DataStores.Queues.QueueNameProcessEmail);
            var msg = receivedMessage == null ? GenericActionMessage.CreateNewQueueMessage() : GenericActionMessage.CreateQueueMessageFromExistingMessage(receivedMessage);
            await queueRef.AddMessageAsync(msg);
            Dependencies.DiagnosticLogging.Info("Mail Sanitised Acknowledgement lodged.");
        }
    }
}
