using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using QueryMailApiFunction.Data;
using QueryMailApiFunction.Extensions;

namespace QueryMailApiFunction
{
    public static class QueryStatusApi
    {
        [FunctionName("QueryStatus")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, ILogger log)
        {
            var coreDependencies = CoreDependencies.Setup(log);
            var statusRepo = new StatusRepository(coreDependencies);

            // Setup dependencies and invoke main processing component.
            var engine = new QueryEngine(coreDependencies, new QueryApiRepository(coreDependencies),statusRepo);
            var apiResponse = await engine.GetStatusAsync();
            return apiResponse.ToHttpResponseMessage(req);
        }
    }
}
