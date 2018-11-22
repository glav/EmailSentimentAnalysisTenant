using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMailApiFunction.Data
{
    public interface IQueryApiRepository
    {
        Task<IEnumerable<MailSentimentMessageEntity>> GetMailSentimentAsync(int maxItemsToRetrieve = 50);
    }
}
