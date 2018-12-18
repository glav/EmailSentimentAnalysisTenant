using Core;
using Core.Data;
using MailCollectorFunction.Config;
using MailCollectorFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailCollectorFunction
{
    public class CollectionEngine
    {
        private readonly CoreDependencyInstances _coreDependencies;
        private readonly IMailCollectionRepository _repository;
        private readonly EmailConfiguration _mailConfig;
        private readonly IStatusRepository _statusRepository;
        public CollectionEngine(CoreDependencyInstances coreDependencies, IMailCollectionRepository repository, IStatusRepository statusRepository, EmailConfiguration mailConfig)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;
            _statusRepository = statusRepository;
            _mailConfig = mailConfig;
        }

        public async Task PerformMailCollectionAsync(GenericActionMessage receivedMessage)
        {
            _coreDependencies.DiagnosticLogging.Verbose("MailCollection: Performing Mail Collection:{0}", receivedMessage);

            try
            {
                await _statusRepository.UpdateStatusAsync("Collecting mail");
                var emails = await _repository.CollectMailAsync(_mailConfig);
        
                TrimMailDataIfRequired(emails);
                await _repository.StoreMailAsync(emails);
                await _statusRepository.UpdateStatusAsync($"Collected #{emails.Count} emails");
            }
            catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "MailCollection: Error performing mail collection");
                await _statusRepository.UpdateStatusAsync("Error collecting mail");

            }

            try
            {
                await _repository.LodgeMailCollectedAcknowledgementAsync(receivedMessage);
                _coreDependencies.DiagnosticLogging.Debug("Completed performing mail collection");
            } catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "MailCollection: Could not Lodge mail collection acknowledgement. Manual intervention required");
            }

            return;
        }

        private void TrimMailDataIfRequired(List<RawMailMessageEntity> emails)
        {
            if (emails == null || emails.Count == 0)
            {
                return;
            }
            const int maxSize = 32000;
            // If a field is > 64k then it will fail when attempting to store in storage
            // Note: I know 65530 is not 64k exactly but keeping it just under
            foreach (var mail in emails)
            {
                if (mail.Body.Length > maxSize)
                {
                    _coreDependencies.DiagnosticLogging.Warning("Trimming email Body as exceeds max length");
                    mail.Body = mail.Body.Substring(0, maxSize);
                }
                if (mail.Subject.Length > maxSize)
                {
                    _coreDependencies.DiagnosticLogging.Warning("Trimming email Subject as exceeds max length");
                    mail.Subject = mail.Subject.Substring(0, maxSize);
                }
            }
        }
    }
}
