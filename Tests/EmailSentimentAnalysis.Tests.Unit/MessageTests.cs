using Core;
using Core.Config;
using Core.Data;
using MailSanitiserFunction;
using MailSanitiserFunction.Strategies;
using System;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class MessageTests
    {
        private CoreDependencyInstances _coreDependencies;
        public MessageTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }
        [Fact]
        public void ShouldReadEnvironmentVariable()
        {
            var originalMsg = new GenericActionMessage();
            var serialisedData = originalMsg.ToString();
            var deserialisedMsg = GenericActionMessage.FromString(serialisedData);

            Assert.NotNull(deserialisedMsg);
            Assert.Equal(originalMsg.ActionDateTimeUtc, deserialisedMsg.ActionDateTimeUtc);
            Assert.Equal(originalMsg.CorrelationId, deserialisedMsg.CorrelationId);
        }


    }
}
