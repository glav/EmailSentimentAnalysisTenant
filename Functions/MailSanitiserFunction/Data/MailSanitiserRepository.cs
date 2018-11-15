using Core;
using Core.Data;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailSanitiserFunction.Data
{
    public class MailSanitiserRepository : BaseCloudStorageRepository, IMailSanitiserRepository
    {
        public MailSanitiserRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {
        }

        public async Task StoreSanitisedMailAsync(List<SanitisedMailMessageEntity> mail)
        {
            if (mail == null || mail.Count == 0)
            {
                Dependencies.DiagnosticLogging.Error("Sanitisation: No mail to store.");
            }
            var numMsgs = mail.Count;
            try
            {
                Dependencies.DiagnosticLogging.Verbose($"Sanitisation: {numMsgs} mail messages to store.");
                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameSanitisedMail);

                foreach (var m in mail)
                {
                    var op = TableOperation.Insert(m);
                    var result = await tblRef.ExecuteAsync(op);
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("Sanitisation: Unable to write MailMessage to table storage {m}", m);
                    }
                }
                Dependencies.DiagnosticLogging.Info("Sanitisation: Mail messages stored: #{numMsgs}", numMsgs);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "Sanitisation: Error sending mail list to queue ");
                var baseEx = ex.GetBaseException();
                if (baseEx != null)
                {
                    Dependencies.DiagnosticLogging.Error(baseEx, "Sanitisation: Error sending mail list to queue (Inner/base error)");
                }
            }
        }

        public async Task<List<SanitisedMailMessageEntity>> GetCollectedMailAsync()
        {
            Dependencies.DiagnosticLogging.Info("Sanitisation: Retrieving mail records");

            var results = new List<SanitisedMailMessageEntity>();
            var tblRef = CreateClientTableReference(DataStores.Tables.TableNameCollectMail);
            var qry = new TableQuery<SanitisedMailMessageEntity>();

            TableContinuationToken continuationToken = null;
            do
            {
                // Retrieve a segment (up to 1,000 entities).
                var tableQueryResult =
                    await tblRef.ExecuteQuerySegmentedAsync(qry, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;

                var recordsRead = tableQueryResult.Results.Count;
                Dependencies.DiagnosticLogging.Info("Sanitisation: Mail Records retrieved to sanitise: {recordsRead}", recordsRead);
                if (continuationToken != null)
                {
                    Dependencies.DiagnosticLogging.Verbose("Sanitisation: More mail records are in queue to be read");
                }
                results.AddRange(tableQueryResult.Results);

            } while (continuationToken != null);

            return results;
        }

        public async Task ClearCollectedMailAsync()
        {
            Dependencies.DiagnosticLogging.Info("Sanitisation: Clearing/deleting mail records");

            var results = new List<SanitisedMailMessageEntity>();
            var tblRef = CreateClientTableReference(DataStores.Tables.TableNameCollectMail);
            var qry = new TableQuery<SanitisedMailMessageEntity>();
            int recordsProcessed = 0;

            TableContinuationToken continuationToken = null;
            do
            {
                // Retrieve a segment (up to 1,000 entities).
                var tableQueryResult =
                    await tblRef.ExecuteQuerySegmentedAsync(qry, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;

                if (tableQueryResult.Results == null && tableQueryResult.Results.Count == 0)
                {
                    break;
                }
                tableQueryResult.Results.ForEach(async r =>
                {
                    var op = TableOperation.Delete(r);
                    var result = await tblRef.ExecuteAsync(op);
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("Sanitisation: There were errors clearing the collected mail.");
                    } else { recordsProcessed++;  }
                });

            } while (continuationToken != null);
            Dependencies.DiagnosticLogging.Info("Sanitisation: Cleared collected mail #{recordsProcessed} records deleted.", recordsProcessed);
        }

        public async Task LodgeMailSanitisedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            Dependencies.DiagnosticLogging.Debug("Lodging Mail Sanitised Acknowledgement");
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(DataStores.Queues.QueueNameProcessEmail);
            var msg = receivedMessage == null ? GenericActionMessage.CreateNewQueueMessage() : GenericActionMessage.CreateQueueMessageFromExistingMessage(receivedMessage);
            await queueRef.AddMessageAsync(msg);
            Dependencies.DiagnosticLogging.Info("Mail Sanitised Acknowledgement lodged.");
        }
    }
}
