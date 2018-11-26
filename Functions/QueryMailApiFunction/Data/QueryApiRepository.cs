using Core;
using Core.Data;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMailApiFunction.Data
{
    public class QueryApiRepository : BaseCloudStorageRepository, IQueryApiRepository
    {
        public QueryApiRepository(CoreDependencyInstances coreDependencies) : base(coreDependencies)
        {

        }
        public async Task<IEnumerable<MailSentimentMessageEntity>> GetMailSentimentAsync(int maxItemsToRetrieve = 50)
        {
            Dependencies.DiagnosticLogging.Verbose("QueryApi: Retrieving mail records");

            var results = new List<MailSentimentMessageEntity>();
            var tblRef = CreateClientTableReference(DataStores.Tables.TableNameProcessedMail);
            var qry = new TableQuery<MailSentimentMessageEntity>();

            TableContinuationToken continuationToken = null;
            do
            {
                // Retrieve a segment (up to 1,000 entities).
                var tableQueryResult =
                    await tblRef.ExecuteQuerySegmentedAsync(qry, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;

                var recordsRead = tableQueryResult.Results.Count;
                Dependencies.DiagnosticLogging.Debug("MailProcessor: Mail Records retrieved to analyse: {recordsRead}", recordsRead);
                if (continuationToken != null)
                {
                    Dependencies.DiagnosticLogging.Verbose("MailProcessor: More mail records are in queue to be read");
                }
                results.AddRange(tableQueryResult.Results);

            } while (continuationToken != null);
            var totalRecords = results.Count;
            Dependencies.DiagnosticLogging.Verbose("QueryApi: Total mail records retrieved for API : {totalRecords}", totalRecords);

            return results;
        }
    }
}
