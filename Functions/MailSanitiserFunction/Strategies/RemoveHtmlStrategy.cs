using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MailSanitiserFunction.Strategies
{
    public class RemoveHtmlStrategy : IMailSanitiserStrategy
    {
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseContent(string content)
        {
            return Regex.Replace(content, "<.*?>", String.Empty);
        }
    }
}
