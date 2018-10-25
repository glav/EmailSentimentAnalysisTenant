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

        [Fact]
        public void ShouldReadEnvironmentVariable()
        {
            var coreDependencies = Dependencies.Setup();
            var reader = new EnvironmentValueReader(coreDependencies.DiagnosticLogging);
            var result = reader.GetEnvironmentValueThatIsNotEmpty(new string[] { "PATH", "path" });

            Assert.NotNull(result);
        }

    }
}
