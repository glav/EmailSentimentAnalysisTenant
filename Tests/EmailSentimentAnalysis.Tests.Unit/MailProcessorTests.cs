using Core;
using Core.Config;
using Glav.CognitiveServices.FluentApi.Core;
using MailProcessorFunction.Config;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class MailProcessorTests
    {
        private CoreDependencyInstances _coreDependencies;
        public MailProcessorTests()
        {
            _coreDependencies = CoreDependencies.Setup();
        }
        [Fact]
        public void MailSanitisationShouldProceedThroughEntireWorkflowWhenMessagesAreCollected()
        {
            var testDependencies = new CoreDependencyInstances(_coreDependencies.DiagnosticLogging, new DummyEnvVariableReader());
            var result = AnalysisConfiguration.PopulateConfigFromEnviromentVariables(testDependencies);

            Assert.Equal(LocationKeyIdentifier.WestUs, result.ApiLocation);
        }


    }

    public class DummyEnvVariableReader : IEnvironmentValueReader
    {
        public string GetEnvironmentValueThatIsNotEmpty(string[] environmentVariables, string defaultValue = null)
        {
            return "westus";
        }
    }

}
