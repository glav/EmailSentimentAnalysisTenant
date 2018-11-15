using Core;
using Core.Data;
using Glav.CognitiveServices.FluentApi.TextAnalytic;
using Glav.CognitiveServices.FluentApi;
using Glav.CognitiveServices.FluentApi.Core;
using MailProcessorFunction.Config;
using MailProcessorFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MailProcessorFunction
{
    public class MailProcessingEngine
    {
        private readonly CoreDependencyInstances _coreDependencies;
        private readonly IMailProcessorRepository _repository;
        private readonly AnalysisConfiguration _config;

        public MailProcessingEngine(CoreDependencyInstances coreDependencies, 
            IMailProcessorRepository repository, AnalysisConfiguration config)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;
            _config = config;
        }

        public async Task AnalyseAllMailAsync(GenericActionMessage receivedMessage)
        {
            _coreDependencies.DiagnosticLogging.Info("ProcessMail: Process All Mail");

            try
            {
                var mail = await _repository.GetSanitisedMailAsync();
                if (mail.Count == 0)
                {
                    _coreDependencies.DiagnosticLogging.Info("ProcessMail: Nothing to process");
                    await _repository.LodgeMailProcessorAcknowledgementAsync(receivedMessage);
                    return;
                }
                var apiKey = _config.ApiKey;
                var location = _config.ApiLocation;

                mail.ForEach(async m =>
                {
                    var result = await TextAnalyticConfigurationSettings.CreateUsingConfigurationKeys(apiKey, location)
                        .AddCustomDiagnosticLogging(new SentimentAnalysisLoggingAdapter(_coreDependencies))
                        .UsingHttpCommunication()
                        .WithTextAnalyticAnalysisActions()
                        .AddSentimentAnalysis(m.SanitisedBody)
                        .AddKeyPhraseAnalysis(m.SanitisedBody)
                        .AnalyseAllAsync();
                    m.SentimentClassification = result.SentimentAnalysis.GetResults().First().score;
                    m.SentimentKeyPhrases = string.Join(",", result.KeyPhraseAnalysis.AnalysisResult.ResponseData.documents.First().keyPhrases);
                });
                await _repository.StoreAllAnalysedMailAsync(mail);
                await _repository.ClearSanitisedMailAsync();
                await _repository.LodgeMailProcessorAcknowledgementAsync(receivedMessage);
            }
            catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "ProcessMail: Error attempting to Analyse Mail");
            }
        }

        private async Task AnalyseAllMail(List<AnalysedMailMessageEntity> mailToAnalyse)
        {
            if (mailToAnalyse == null || mailToAnalyse.Count == 0)
            {
                _coreDependencies.DiagnosticLogging.Info("ProcessMail: No mail to analyse");
                return;
            }

            mailToAnalyse.ForEach(m =>
            {
                //var results = TextAnalyticConfigurationSettings.CreateUsingConfigurationKeys()
            });
        }
    }
}
