using Core.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;
using System;
using System.Threading.Tasks;

namespace StorageSetup
{
    public class ResourceCreator
    {
        public ResourceCreator(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
        private CloudTableClient CreateTableClient()
        {
            CloudStorageAccount cloudAcct;

            if (!CloudStorageAccount.TryParse(ConnectionString, out cloudAcct))
            {
                Log.Logger.Error("Unable to parse connection string: {0}", ConnectionString);
                return null;
            }

            return cloudAcct.CreateCloudTableClient();

        }
        public async Task<bool> CreateTables()
        {
            var client = CreateTableClient();

            try
            {
                foreach (var tblName in DataStores.Tables.FullTableList)
                {

                    var tblRef = client.GetTableReference(tblName);
                    var tableExists = await tblRef.ExistsAsync();
                    if (!tableExists)
                    {
                        Log.Logger.Information("Creating table [{0}]", tblName);
                        await tblRef.CreateIfNotExistsAsync();
                    }
                    else
                    {
                        Log.Logger.Information("Table [{0}] exists, not creating", tblName);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Unable to create tables");
                return false;
            }

        }
    }
}
