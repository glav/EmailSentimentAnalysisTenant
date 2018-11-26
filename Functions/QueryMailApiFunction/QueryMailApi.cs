using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using QueryMailApiFunction.Data;

namespace QueryMailApiFunction
{
    public static class QueryMailApi
    {
        [FunctionName("QueryMail")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, ILogger log)
        {
            var coreDependencies = CoreDependencies.Setup(log);

            coreDependencies.DiagnosticLogging.Verbose("QueryMail: HTTP trigger function executed at: {Now} UTC", DateTime.UtcNow);

            // Setup dependencies and invoke main processing component.
            var engine = new QueryEngine(coreDependencies, new QueryApiRepository(coreDependencies));
            return req.CreateResponse(await engine.GetMailSentimentListAsync());
        }
    }
}
