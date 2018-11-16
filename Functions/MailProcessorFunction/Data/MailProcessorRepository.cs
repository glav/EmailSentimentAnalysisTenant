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
                    Dependencies.DiagnosticLogging.Verbose("MailProcessor: More mail records are in queue to be read");
                }
                results.AddRange(tableQueryResult.Results);

            } while (continuationToken != null);
            var totalRecords = results.Count;
            Dependencies.DiagnosticLogging.Info("MailProcessor: Total mail records retrieved to analyse: {totalRecords}", totalRecords);

            return results;
        }

        public Task StoreAllAnalysedMailAsync(List<AnalysedMailMessageEntity> analysedMail)
        {
            Dependencies.DiagnosticLogging.Error("MailProcessor: StoreAllAnalysedMailAsync not implemented");
            return Task.FromResult(0);
        }

        public async Task ClearSanitisedMailAsync()
        {
            await ClearAllDataFromStorageAsync<AnalysedMailMessageEntity>(DataStores.Tables.TableNameSanitisedMail, "MailProcessor");
        }

        public async Task LodgeMailProcessorAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            await LodgeAcknowledgementMessageAsync(receivedMessage, "MailProcessor", DataStores.Queues.QueueNameTriggerEmail);
        }
    }
}
