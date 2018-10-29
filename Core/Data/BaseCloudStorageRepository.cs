using Core.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data
{
    public abstract class BaseCloudStorageRepository
    {
        public BaseCloudStorageRepository(CoreDependencyInstances dependencies)
        {
            Dependencies = dependencies;
        }
        protected CoreDependencyInstances Dependencies { get; private set; }
        protected CloudTable CreateClientTableReference(string tableName)
        {
            var acct = CreateStorageAccountReference();
            var client = acct.CreateCloudTableClient();
            return client.GetTableReference(tableName);
        }

        protected CloudStorageAccount CreateStorageAccountReference()
        {
            CloudStorageAccount cloudAcct;
            var connString = Dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { ConfigKeys.StorageConnectionString });

            if (!CloudStorageAccount.TryParse(connString, out cloudAcct))
            {
                Dependencies.DiagnosticLogging.Fatal("Unable to parse connection string: {0}", connString);
                throw new Exception($"Unable to parse connection string: {connString}");
            }

            return cloudAcct;

        }

    }
}
