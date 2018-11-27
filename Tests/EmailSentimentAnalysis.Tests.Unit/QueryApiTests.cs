using Core;
using Core.Config;
using Core.Data;
using MailCollectorFunction;
using MailSanitiserFunction;
using MailSanitiserFunction.Strategies;
using QueryMailApiFunction;
using QueryMailApiFunction.Data;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class QueryApiTests
    {
        private CoreDependencyInstances _coreDependencies;
        public QueryApiTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }

        [Fact]
        public async Task QueryApiShouldReturnBadRequestOnError()
        {
            var repo = new DummyQueryRepo(true);
            var engine = new QueryEngine(_coreDependencies, repo);
            var response = await engine.GetMailSentimentListAsync();

            Assert.Equal(1, repo.InvocationCount);
            Assert.Equal(HttpStatusCode.BadRequest, response.ErrorCode);
      }

        [Fact]
        public async Task QueryApiShouldReturnDataWhenNoError()
        {
            var repo = new DummyQueryRepo(false);
            var engine = new QueryEngine(_coreDependencies, repo);
            var response = await engine.GetMailSentimentListAsync();

            Assert.Equal(1, repo.InvocationCount);
            Assert.False(response.HasError);
            Assert.NotNull(response.ResponseData);
            Assert.NotEmpty(response.ResponseData);
        }

    }

    public class DummyQueryRepo : IQueryApiRepository
    {
        private bool _simulateError = false;
        public DummyQueryRepo(bool simulateError = false)
        {
            _simulateError = simulateError;
        }
        public int InvocationCount = 0;
        public Task<IEnumerable<MailSentimentMessageEntity>> GetMailSentimentAsync(int maxItemsToRetrieve = 50)
        {
            InvocationCount++;
            if (_simulateError)
            {
                throw new System.Exception("Flux capacitor exceeded widget centrifiscal values");
            }
            var dummyData = new List<MailSentimentMessageEntity>();
            dummyData.Add(new MailSentimentMessageEntity { PartitionKey = "1", RowKey = "2", Body = "3" });
            return Task.FromResult<IEnumerable<MailSentimentMessageEntity>>(dummyData);
        }
    }

}
