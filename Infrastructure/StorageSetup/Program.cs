using Microsoft.WindowsAzure.Storage;
using Serilog;
using Serilog.Events;
using StorageSetup.Config;
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

            var connString = ConfigReader.ConnectionStringStorageAccount;
            Log.Logger.Information("Connection string: {0}", connString);

            CloudStorageAccount cloudAcct;
            
            if (!CloudStorageAccount.TryParse(connString, out cloudAcct))
            {
                Log.Logger.Error("Unable to parse connection string: {0}", connString);
                return -1;
            }

            var creator = new ResourceCreator();
            return creator.CreateTables(cloudAcct).Result ? 0 : -1;
        }

    }
}
