using Core;
using Core.Data;
using MailSanitiserFunction.Data;
using MailSanitiserFunction.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSanitiserFunction
{
    public class MailSanitiserEngine
    {
        private readonly CoreDependencyInstances _coreDependencies;
        private readonly IMailSanitiserRepository _repository;

        public MailSanitiserEngine(CoreDependencyInstances coreDependencies, IMailSanitiserRepository repository)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;

            SetupStrategies();
        }

        private void SetupStrategies()
        {
            SanitiserStrategies = new List<IMailSanitiserStrategy>();
            SanitiserStrategies.Add(new RemoveAllButBodyStrategy());
            SanitiserStrategies.Add(new RemoveHtmlCommentsStrategy());
            SanitiserStrategies.Add(new RemoveHtmlStrategy());
            SanitiserStrategies.Add(new RemoveEncodedCharactersStrategy());
        }

        public List<IMailSanitiserStrategy> SanitiserStrategies { get; private set; }

        public string SanitiseForAllContentTypes(string content)
        {
            return SanitiseContent(content, SanitiseContentType.Html & SanitiseContentType.PlainText);

        }

        public async Task SanitiseMailAsync(GenericActionMessage receivedMessage)
        {
            _coreDependencies.DiagnosticLogging.Info("SanitiseMail: Sanitise All Mail");

            try
            {
                var mail = await _repository.GetCollectedMailAsync();
                if (mail.Count == 0)
                {
                    _coreDependencies.DiagnosticLogging.Info("SanitiseMail: Nothing to sanitise");
                    await _repository.LodgeMailSanitisedAcknowledgementAsync(receivedMessage);
                    return;
                }
                mail.ForEach(m =>
                {
                    m.SanitisedBody = SanitiseForAllContentTypes(m.Body);
                });
                await _repository.StoreSanitisedMailAsync(mail);
                await _repository.ClearCollectedMailAsync();
                await _repository.LodgeMailSanitisedAcknowledgementAsync(receivedMessage);
            } catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "Error attempting to Sanitise Mail");
            }
        }


        public string SanitiseContent(string content, SanitiseContentType contentType)
        {
            if (SanitiserStrategies.Count == 0)
            {
                return content;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }

            var buffer = new StringBuilder(content);
            var tmpContent = new StringBuilder();

            //bitwise enum check, can be both html or plain text
            var strategies = SanitiserStrategies.Where(s => s.ContentTypeSupported.HasFlag(contentType)).ToList();
            strategies.ForEach(s =>
            {
                tmpContent.Append(s.SanitiseContent(buffer.ToString()));
                buffer.Clear();
                buffer.Append(tmpContent);
                tmpContent.Clear();
            });

            return buffer.ToString();
        }
    }
}
