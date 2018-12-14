using Core.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public abstract class BaseCloudStorageRepository
    {
        public BaseCloudStorageRepository(CoreDependencyInstances dependencies)
        {
            Dependencies = dependencies;
        }
        protected CoreDependencyInstances Dependencies { get; private set; }
        protected CloudTable CreateClientTableReference(string tableName)
        {
            var acct = CreateStorageAccountReference();
            var client = acct.CreateCloudTableClient();
            return client.GetTableReference(tableName);
        }

        protected async Task ClearAllDataFromStorageAsync<T>(string tableName, string processName) where T : TableEntity, new()
        {
            Dependencies.DiagnosticLogging.Verbose($"{processName}: Clearing/deleting records");

            var results = new List<T>();
            var tblRef = CreateClientTableReference(tableName);
            var qry = new TableQuery<T>();
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
                    r.ETag = "*";
                    var op = TableOperation.Delete(r);
                    var result = await tblRef.ExecuteAsync(op);
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error($"{processName}: There were errors clearing records from storage table: {tableName} .");
                    }
                    else { recordsProcessed++; }
                });

            } while (continuationToken != null);
            Dependencies.DiagnosticLogging.Info($"{processName}: Cleared records from storage table {tableName}");
        }


        protected CloudStorageAccount CreateStorageAccountReference()
        {
            CloudStorageAccount cloudAcct;
            var connString = Dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { ConfigKeys.StorageConnectionString });

            if (!CloudStorageAccount.TryParse(connString, out cloudAcct))
            {
                Dependencies.DiagnosticLogging.Fatal("Unable to parse connection string: {0}", connString);
                throw new Exception($"Unable to parse connection string: {connString}");
            }

            return cloudAcct;

        }

        protected async Task LodgeAcknowledgementMessageAsync(GenericActionMessage receivedMessage, string processName, string queueName)
        {
            Dependencies.DiagnosticLogging.Debug($"{processName}: Lodging acknowledgement message");
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(queueName);
            var msg = receivedMessage == null ? GenericActionMessage.CreateNewQueueMessage() : GenericActionMessage.CreateQueueMessageFromExistingMessage(receivedMessage);
            await queueRef.AddMessageAsync(msg);
            Dependencies.DiagnosticLogging.Verbose($"{processName}: Acknowledgement message lodged.");
        }


    }
}
