using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public class StatusRepository : BaseCloudStorageRepository, IStatusRepository
    {
        public StatusRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {
        }
        public async Task UpdateStatusAsync(string message)
        {
            try
            {
                Dependencies.DiagnosticLogging.Verbose("UpdateStatus: Updating status");

                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameStatusUpdate);
                var op = TableOperation.InsertOrReplace(new StatusUpdateEntity { Message = message });
                var result = await tblRef.ExecuteAsync(op);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Fatal(ex, "UpdateStatus: Error updating status");

            }
        }

        public async Task ClearStatusAsync()
        {
            try
            {
                Dependencies.DiagnosticLogging.Verbose("UpdateStatus: Clearing status");

                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameStatusUpdate);
                var op = TableOperation.Delete(new StatusUpdateEntity());
                var result = await tblRef.ExecuteAsync(op);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Fatal(ex, "UpdateStatus: Error clearing status");
            }
        }
        public async Task<StatusUpdateEntity> GetStatusAsync()
        {
            try
            {
                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameStatusUpdate);
                var op = TableOperation.Retrieve(DataStores.Tables.StatusTablePartitionKey, DataStores.Tables.StatusTableRowKey);
                var result = await tblRef.ExecuteAsync(op);
                return result.Result as StatusUpdateEntity;
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Fatal(ex, "UpdateStatus: Error retrieving status");

            }
            return null;

        }
    }
}
