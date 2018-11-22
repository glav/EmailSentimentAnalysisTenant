using Core;
using Core.Data;
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
        public Task<IEnumerable<MailSentimentMessageEntity>> GetMailSentimentAsync(int maxItemsToRetrieve = 50)
        {
            throw new NotImplementedException();
        }
    }
}
