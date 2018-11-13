using Core;
using Core.Data;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailProcessorFunction.Data
{
    public class MailProcessorRepository : BaseCloudStorageRepository, IMailProcessorRepository
    {
        public MailProcessorRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {

        }

        public async Task<List<AnalysedMailMessageEntity>> GetSanitisedMailAsync()
        {
            Dependencies.DiagnosticLogging.Info("MailProcessor: Retrieving mail records");

            var results = new List<AnalysedMailMessageEntity>();
            var tblRef = CreateClientTableReference(DataStores.Tables.TableNameSanitisedMail);
            var qry = new TableQuery<AnalysedMailMessageEntity>();

            TableContinuationToken continuationToken = null;
            do
            {
                // Retrieve a segment (up to 1,000 entities).
                var tableQueryResult =
                    await tblRef.ExecuteQuerySegmentedAsync(qry, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;

                var recordsRead = tableQueryResult.Results.Count;
                Dependencies.DiagnosticLogging.Debug("MailProcessor: Mail Records retrieved to analyse: {recordsRead}", recordsRead);
                if (continuationToken != null)
                {
                    Dependencies.DiagnosticLogging.Verbose("More mail records are in queue to be read");
                }
                results.AddRange(tableQueryResult.Results);

            } while (continuationToken != null);
            var totalRecords = results.Count;
            Dependencies.DiagnosticLogging.Info("MailProcessor: Total mail records retrieved to analyse: {totalRecords}", totalRecords);

            return results;
        }

        public async Task LodgeMailProcessorAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            Dependencies.DiagnosticLogging.Debug("MailProcessor: Lodging Mail processed Acknowledgement");
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(DataStores.Queues.QueueNameTriggerEmail);
            var msg = receivedMessage == null ? GenericActionMessage.CreateNewQueueMessage() : GenericActionMessage.CreateQueueMessageFromExistingMessage(receivedMessage);
            await queueRef.AddMessageAsync(msg);
            Dependencies.DiagnosticLogging.Info("MailProcessor: Mail processor acknowledgement lodged.");
        }
    }
}
