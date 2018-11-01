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


            var creator = new ResourceCreator(connString);
            return creator.CreateTables().Result ? 0 : -1;
        }

    }
}
