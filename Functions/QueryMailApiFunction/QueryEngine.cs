using Core;
using QueryMailApiFunction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<ApiResponse<IEnumerable<QueryApiMessage>>> GetMailSentimentListAsync()
        {
            _coreDependencies.DiagnosticLogging.Verbose("QueryApi: Attempting to Get mail sentiment list");

            try
            {
                var result = await _repository.GetMailSentimentAsync();
                var numMails = result.Count();
                _coreDependencies.DiagnosticLogging.Info("QueryApi: Retrieved {@numMails} from datastore.", numMails);

                return new ApiResponse<IEnumerable<QueryApiMessage>>(result.ToApiMessages());
            } catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex,"QueryApi: Error attempting to get mail sentiment list");
                return new ApiResponse<IEnumerable<QueryApiMessage>>(HttpStatusCode.BadRequest, "Error querying the mail data source");
            }
        }
    }
}
