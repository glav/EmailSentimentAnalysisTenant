﻿using CollectMailScheduler.Config;
using Core;
using Core.Data;
using Microsoft.WindowsAzure.Storage.Queue;
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
                var tblRef = CreateClientTableReference(FunctionConstants.TableNameMailSchedulerStatus);
                var op = TableOperation.Delete(new MailSchedulerEntity());
                var result = await tblRef.ExecuteAsync(op);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "Error clearing mail operation progress");
            }
        }

        public async Task<bool> IsMailOperationInProgressAsync()
        {
            Dependencies.DiagnosticLogging.Info("IsMailOperationInprogress");
            var tblRef = CreateClientTableReference(FunctionConstants.TableNameMailSchedulerStatus);
            try
            {
                var op = TableOperation.Retrieve(FunctionConstants.TablePartitionKey, FunctionConstants.TableRowKey);
                var result = await tblRef.ExecuteAsync(op);
                return result != null && result.HttpStatusCode < 300;
            } catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "Error checking if Mail Operation in progress");
            }
            return false;
        }

        public async Task SetMailOperationToInProgressAsync()
        {
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(FunctionConstants.QueueNameCollectEmail);

            try
            {
                await queueRef.AddMessageAsync(GenericActionMessage.CreateNewQueueMessage());
                Dependencies.DiagnosticLogging.Info("Email collection trigger message sent");

                var tblRef = CreateClientTableReference(FunctionConstants.TableNameMailSchedulerStatus);
                var op = TableOperation.Insert(new MailSchedulerEntity());
                var result = await tblRef.ExecuteAsync(op);
            } catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "Error setting mail operation progress");
            }
        }

    }
}
