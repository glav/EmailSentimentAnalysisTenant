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
    public class MailCollectionTests
    {
        private CoreDependencyInstances _coreDependencies;
        public MailCollectionTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }
        [Fact]
        public async Task MailCollectionShouldLodgeCollectionCompleteAcknowledgementEvenIfMailFailure()
        {
            var repo = new DummyCollectionRepo(TestFlag.BlowUpOnCollection);
            var engine = new CollectionEngine(_coreDependencies, repo,new DummyStatusRepo(), null);

            await engine.PerformMailCollectionAsync(new GenericActionMessage());

            Assert.Equal(1, repo.CollectMailCount);
            Assert.Equal(0, repo.StoreMailCount);
            Assert.Equal(1, repo.LodgeAcknowledgementCount);
      }

        [Fact]
        public async Task MailCollectionShouldLodgeCollectionCompleteAcknowledgementEvenIfStorageFailure()
        {
            var repo = new DummyCollectionRepo(TestFlag.BlowUpOnStoring);
            var engine = new CollectionEngine(_coreDependencies, repo, new DummyStatusRepo(), null);

            await engine.PerformMailCollectionAsync(new GenericActionMessage());

            Assert.Equal(1, repo.CollectMailCount);
            Assert.Equal(1, repo.StoreMailCount);
            Assert.Equal(1, repo.LodgeAcknowledgementCount);
        }

    }

}
