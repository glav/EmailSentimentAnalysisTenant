using Microsoft.WindowsAzure.Storage;
using Serilog;
using System;
using System.Threading.Tasks;

namespace StorageSetup
{
    public class ResourceCreator
    {
        public async Task<bool> CreateTables(CloudStorageAccount cloudAcct)
        {
            string[] tables = { "EmailSentimentCollectMail" };
            var client = cloudAcct.CreateCloudTableClient();

            try
            {
                foreach (var tblName in tables)
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
