﻿using Core;
using Core.Data;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<bool> StoreAllAnalysedMailAsync(List<AnalysedMailMessageEntity> analysedMail)
        {
            if (analysedMail == null || analysedMail.Count == 0)
            {
                Dependencies.DiagnosticLogging.Verbose("MailProcessor: No analysed email to store, exiting.");
                return true;
            }
            var numMsgs = analysedMail.Count;
            try
            {
                Dependencies.DiagnosticLogging.Verbose("MailProcessor: {numMsgs} mail messages to store.",numMsgs);
                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameProcessedMail);

                foreach (var m in analysedMail)
                {
                    var op = TableOperation.Insert(m);
                    var result = await tblRef.ExecuteAsync(op);
                    var resultCode = result.HttpStatusCode;
                    if (resultCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("MailProcessor: Unable to write analysed messages to table storage status code: {resultCode}",resultCode);
                    }
                }
                Dependencies.DiagnosticLogging.Info("MailProcessor: Analysed messages stored: #{numMsgs}", numMsgs);
                return true;
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "MailProcessor: Error storing analysed mail list");
                var baseEx = ex.GetBaseException();
                if (baseEx != null)
                {
                    Dependencies.DiagnosticLogging.Error(baseEx, "MailProcessor: Error storing analysed mail list (Inner/base error)");
                }
            }
            return false;
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
