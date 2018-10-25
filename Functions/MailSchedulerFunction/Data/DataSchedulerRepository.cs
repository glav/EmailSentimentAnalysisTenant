using CollectMailScheduler.Config;
using Core;
using Core.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSchedulerFunction.Data
{
    class DataSchedulerRepository : IDataSchedulerRepository
    {
        private readonly CoreDependencyInstances _dependencies;

        public DataSchedulerRepository(CoreDependencyInstances dependencies)
        {
            _dependencies = dependencies;
        }
        public void ClearMailOperationProgress()
        {
            try
            {
                var tblRef = CreateClientTableReference();
                var op = TableOperation.Delete(new MailSchedulerEntity());
                var result = tblRef.ExecuteAsync(op).Result;
            }
            catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error clearing mail operation progress");
            }
        }

        public bool IsMailOperationInProgress()
        {
            _dependencies.DiagnosticLogging.Info("IsMailOperationInprogress");
            var tblRef = CreateClientTableReference();
            try
            {
                var op = TableOperation.Retrieve(FunctionConfig.TablePartitionKey, FunctionConfig.TableRowKey);
                var result = tblRef.ExecuteAsync(op).Result;
                return result != null;
            } catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error checking if Mail Operation in progress");
            }
            return false;
        }

        public void SetMailOperationToInProgress()
        {
            try
            {
                var tblRef = CreateClientTableReference();
                var op = TableOperation.Insert(new MailSchedulerEntity());
                var result = tblRef.ExecuteAsync(op).Result;
            } catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error setting mail operation progress");
            }
        }

        private CloudTable CreateClientTableReference()
        {
            CloudStorageAccount cloudAcct;
            var connString = _dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { ConfigKeys.StorageConnectionString });

            if (!CloudStorageAccount.TryParse(connString, out cloudAcct))
            {
                _dependencies.DiagnosticLogging.Fatal("Unable to parse connection string: {0}", connString);
                throw new Exception($"Unable to parse connection string: {connString}");
            }
            var client = cloudAcct.CreateCloudTableClient();
            return client.GetTableReference(FunctionConfig.TableNameCollectMail);

        }
    }
}
