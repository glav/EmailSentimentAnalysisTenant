﻿using Core;
using Core.Data;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace MailSchedulerFunction.Data
{
    class DataSchedulerRepository : BaseCloudStorageRepository, IDataSchedulerRepository
    {
        public DataSchedulerRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {
        }
        public async Task ClearMailOperationProgressAsync()
        {
            try
            {
                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameMailSchedulerStatus);
                var op = TableOperation.Delete(new MailSchedulerEntity());
                var result = await tblRef.ExecuteAsync(op);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "MailScheduler: Error clearing mail operation progress");
            }
        }

        public async Task<bool> IsMailOperationInProgressAsync()
        {
            var tblRef = CreateClientTableReference(DataStores.Tables.TableNameMailSchedulerStatus);
            var op = TableOperation.Retrieve(DataStores.Tables.SchedulerTablePartitionKey, DataStores.Tables.SchedulerTableRowKey);
            var result = await tblRef.ExecuteAsync(op);
            return result != null && result.HttpStatusCode < 300;
        }

        public async Task SetMailOperationToInProgressAsync()
        {
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(DataStores.Queues.QueueNameCollectEmail);

            try
            {
                await queueRef.AddMessageAsync(GenericActionMessage.CreateNewQueueMessage());
                Dependencies.DiagnosticLogging.Verbose("MailScheduler: Email collection trigger message sent");

                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameMailSchedulerStatus);
                var op = TableOperation.Insert(new MailSchedulerEntity());
                var result = await tblRef.ExecuteAsync(op);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "MailScheduler: Error setting mail operation progress");
            }
        }

    }
}
