using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace StorageSetup
{
    class Program
    {
        static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

            Log.Logger.Information("Connection string: {0}", Config.ConnectionStringStorageAccount);

            CloudStorageAccount cloudAcct;
            if (!CloudStorageAccount.TryParse(Config.ConnectionStringStorageAccount,out cloudAcct))
            {
                Log.Logger.Error("Unable to parse connection string: {0}",Config.ConnectionStringStorageAccount}");
            }

            return CreateTables(cloudAcct).Result ? 0 : -1;
        }

        private async static Task<bool> CreateTables(CloudStorageAccount cloudAcct)
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
                    } else
                    {
                        Log.Logger.Information("Table [{0}] exists, not creating", tblName);

                    }
                }
                return true;
            } catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Unable to create tables");
                return false;
            }

        }
    }
}
