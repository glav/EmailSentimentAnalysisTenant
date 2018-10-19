using System;
using System.Collections.Generic;
using System.Text;

namespace MailSanitiserFunction.Strategies
{
    public class RemoveAllButBodyStrategy : IMailSanitiserStrategy
    {
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseContent(string content)
        {
            var startIndex = content.IndexOf("<body>", 0, StringComparison.InvariantCultureIgnoreCase);
            if (startIndex < 0) return content;
            startIndex += "<body>".Length;

            var endIndex = content.IndexOf("</body>", 0, StringComparison.InvariantCultureIgnoreCase);
            if (endIndex < 0) return content;

            return content.Substring(startIndex,endIndex-startIndex);


        }
    }
}
