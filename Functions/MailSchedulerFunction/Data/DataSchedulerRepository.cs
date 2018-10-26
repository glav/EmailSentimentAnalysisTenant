using CollectMailScheduler.Config;
using Core;
using Core.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailSchedulerFunction.Data
{
    class DataSchedulerRepository : IDataSchedulerRepository
    {
        private readonly CoreDependencyInstances _dependencies;

        public DataSchedulerRepository(CoreDependencyInstances dependencies)
        {
            _dependencies = dependencies;
        }
        public async Task ClearMailOperationProgressAsync()
        {
            try
            {
                var tblRef = CreateClientTableReference();
                var op = TableOperation.Delete(new MailSchedulerEntity());
                var result = await tblRef.ExecuteAsync(op);
            }
            catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error clearing mail operation progress");
            }
        }

        public async Task<bool> IsMailOperationInProgressAsync()
        {
            _dependencies.DiagnosticLogging.Info("IsMailOperationInprogress");
            var tblRef = CreateClientTableReference();
            try
            {
                var op = TableOperation.Retrieve(FunctionConfig.TablePartitionKey, FunctionConfig.TableRowKey);
                var result = await tblRef.ExecuteAsync(op);
                return result != null && result.HttpStatusCode < 300;
            } catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error checking if Mail Operation in progress");
            }
            return false;
        }

        public async Task SetMailOperationToInProgressAsync()
        {
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(FunctionConfig.QueueNameCollectEmail);

            try
            {
                queueRef.AddMessageAsync(new CloudQueueMessage(DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss"))).Wait();
                _dependencies.DiagnosticLogging.Info("Email collection trigger message sent");

                var tblRef = CreateClientTableReference();
                var op = TableOperation.Insert(new MailSchedulerEntity());
                var result = await tblRef.ExecuteAsync(op);
            } catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error setting mail operation progress");
            }
        }

        private CloudTable CreateClientTableReference()
        {
            var acct = CreateStorageAccountReference();
            var client = acct.CreateCloudTableClient();
            return client.GetTableReference(FunctionConfig.TableNameCollectMail);

        }
        private CloudStorageAccount CreateStorageAccountReference()
        {
            CloudStorageAccount cloudAcct;
            var connString = _dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { ConfigKeys.StorageConnectionString });

            if (!CloudStorageAccount.TryParse(connString, out cloudAcct))
            {
                _dependencies.DiagnosticLogging.Fatal("Unable to parse connection string: {0}", connString);
                throw new Exception($"Unable to parse connection string: {connString}");
            }

            return cloudAcct;

        }
    }
}
