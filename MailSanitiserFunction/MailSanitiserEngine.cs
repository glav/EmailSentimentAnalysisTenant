using MailSanitiserFunction.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailSanitiserFunction
{
    public class MailSanitiserEngine
    {
        public MailSanitiserEngine()
        {
            SanitiserStrategies = new List<IMailSanitiserStrategy>();
            SanitiserStrategies.Add(new RemoveHtmlStrategy());
        }

        public List<IMailSanitiserStrategy> SanitiserStrategies { get; private set; }

        public string Sanitise(string content, SanitiseContentType contentType)
        {
            if (SanitiserStrategies.Count == 0)
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
