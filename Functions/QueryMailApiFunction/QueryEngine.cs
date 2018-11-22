using Core;
using QueryMailApiFunction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMailApiFunction
{
    public class QueryEngine
    {
        private readonly IQueryApiRepository _repository;
        private readonly CoreDependencyInstances _coreDependencies;

        public QueryEngine(CoreDependencyInstances coreDependencies, IQueryApiRepository repository)
        {
            _repository = repository;
            _coreDependencies = coreDependencies;
        }

        public async Task<IEnumerable<QueryApiMessage>> GetMailSentimentListAsync()
        {
            _coreDependencies.DiagnosticLogging.Verbose("QueryApi: Attempting to Get mail sentiment list");

            var result = await _repository.GetMailSentimentAsync();
            return result.ToApiMessages();
        }
    }
}
