using Core;
using Core.Config;
using Glav.CognitiveServices.FluentApi.Core;

namespace MailProcessorFunction.Config
{
    public class AnalysisConfiguration
    {
        public string ApiKey { get; set; }
        public LocationKeyIdentifier ApiLocation { get; set; }
        public static AnalysisConfiguration PopulateConfigFromEnviromentVariables(CoreDependencyInstances dependencies)
        {
            var mailConfig = new AnalysisConfiguration();


            mailConfig.ApiKey =
                dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "APIKey" });

            var locValue = dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "APILocation" });
            mailConfig.ApiLocation = (LocationKeyIdentifier)System.Enum.Parse(typeof(LocationKeyIdentifier),locValue,true);

            return mailConfig;
        }
    }
}
