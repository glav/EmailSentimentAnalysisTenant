using Core;
using Core.Config;
using MailSanitiserFunction;
using MailSanitiserFunction.Strategies;
using System;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class ConfigTests
    {
        private CoreDependencyInstances _coreDependencies;
        public ConfigTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }
        [Fact]
        public void ShouldReadEnvironmentVariable()
        {
            var reader = new EnvironmentValueReader(_coreDependencies.DiagnosticLogging);
            var result = reader.GetEnvironmentValueThatIsNotEmpty(new string[] { "PATH", "path" });

            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldNotReadNonExistentEnvironmentVariable()
        {
            var reader = new EnvironmentValueReader(_coreDependencies.DiagnosticLogging);
            var result = reader.GetEnvironmentValueThatIsNotEmpty(new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });

            Assert.Null(result);
        }

    }
}
