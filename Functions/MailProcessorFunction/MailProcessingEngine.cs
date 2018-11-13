using Core;
using Core.Data;
using MailProcessorFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailProcessorFunction
{
    public class MailProcessingEngine
    {
        private readonly CoreDependencyInstances _coreDependencies;
        private readonly IMailProcessorRepository _repository;

        public MailProcessingEngine(CoreDependencyInstances coreDependencies, IMailProcessorRepository repository)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;
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
                    return;
                }
                //mail.ForEach(m =>
                //{
                //    m.SanitisedBody = SanitiseForAllContentTypes(m.Body);
                //});
                //await _repository.StoreSanitisedMailAsync(mail);
                //await _repository.ClearCollectedMailAsync();
                await _repository.LodgeMailProcessorAcknowledgementAsync(receivedMessage);
            }
            catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "ProcessMail: Error attempting to Sanitise Mail");
            }
        }
    }
}
