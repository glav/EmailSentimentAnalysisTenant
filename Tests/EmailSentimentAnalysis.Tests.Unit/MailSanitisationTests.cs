using Core;
using Core.Config;
using Core.Data;
using MailCollectorFunction;
using MailSanitiserFunction;
using MailSanitiserFunction.Strategies;
using System.Threading.Tasks;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class MailSanitisationTests
    {
        private CoreDependencyInstances _coreDependencies;
        public MailSanitisationTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }
        [Fact]
        public async Task MailSanitisationShouldProceedThroughEntireWorkflowWhenMessagesAreCollected()
        {
            var repo = new DummySanitiserRepo(2);
            var engine = new MailSanitiserEngine(_coreDependencies, repo);
            var msg = new GenericActionMessage();
            await engine.SanitiseMailAsync(msg);

            Assert.Equal(1, repo.MailCollectionCount);
            Assert.Equal(1, repo.StoreSanitisedMaiLCount);
            Assert.Equal(1, repo.LodgeMailAcknowledgementCount);
            Assert.Equal(msg, repo.ActionMessageRecentlyLodged);

      }

        [Fact]
        public async Task MailSanitisationShouldOnlyProceedThroughFirstStepOfWorkflowWhenNoMailCollected()
        {
            var repo = new DummySanitiserRepo(0);
            var engine = new MailSanitiserEngine(_coreDependencies, repo);
            var msg = new GenericActionMessage();
            await engine.SanitiseMailAsync(msg);

            Assert.Equal(1, repo.MailCollectionCount);
            Assert.Equal(0, repo.StoreSanitisedMaiLCount);
            Assert.Equal(0, repo.LodgeMailAcknowledgementCount);

        }


    }

}
