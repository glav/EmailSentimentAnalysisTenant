﻿using Core;
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

                await AnalyseAllMail(mail);
                var success = await _repository.StoreAllAnalysedMailAsync(mail);
                if (success)
                {
                    await _repository.ClearSanitisedMailAsync();
                }
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

            var apiKey = _config.ApiKey;
            var location = _config.ApiLocation;

            foreach (var m in mailToAnalyse)
            {
                var analysis = TextAnalyticConfigurationSettings.CreateUsingConfigurationKeys(apiKey, location)
                    .AddCustomDiagnosticLogging(new SentimentAnalysisLoggingAdapter(_coreDependencies))
                    .UsingHttpCommunication()
                    .WithTextAnalyticAnalysisActions();

                var sentences = m.SanitisedBody.SplitTextIntoSentences();
                foreach (var sentence in sentences)
                {
                    analysis.AddSentimentAnalysis(sentence);
                    analysis.AddKeyPhraseAnalysis(sentence);
                }

                var result = await analysis.AnalyseAllAsync();

                if (!result.SentimentAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
                {
                    var message = result.SentimentAnalysis.AnalysisResult.ResponseData.errors != null ? result.SentimentAnalysis.AnalysisResult.ResponseData.errors.First().message : result.SentimentAnalysis.AnalysisResult.ApiCallResult.Data;
                    _coreDependencies.DiagnosticLogging.Error("ProcessMail: Error processing SentimentAnalysis results: [{message}]", message);
                } else
                {
                    m.SentimentClassification = result.SentimentAnalysis.GetResults().Average(s => s.score);
                }
                if (!result.KeyPhraseAnalysis.AnalysisResult.ActionSubmittedSuccessfully)
                {
                    var message = result.KeyPhraseAnalysis.AnalysisResult.ResponseData.errors != null ? result.KeyPhraseAnalysis.AnalysisResult.ResponseData.errors.First().message : result.KeyPhraseAnalysis.AnalysisResult.ApiCallResult.Data;
                    _coreDependencies.DiagnosticLogging.Error("ProcessMail: Error processing KeyphraseAnalysis results: [{message}]", message);
                } else
                {
                    m.SentimentKeyPhrases = string.Join(",", result.KeyPhraseAnalysis.AnalysisResult.ResponseData?.documents?.Select(s => s.keyPhrases));
                }
                
                m.AnalysedTimestampUtc = DateTime.UtcNow;
            }
        }
    }
}
