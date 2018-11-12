﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MailSanitiserFunction.Strategies
{
    public class RemoveAllButBodyStrategy : IMailSanitiserStrategy
    {
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseContent(string content)
        {
            var startIndex = content.IndexOf("<body", 0, StringComparison.InvariantCultureIgnoreCase);
            if (startIndex < 0) return content;

            var endIndex = content.IndexOf("</body>", 0, StringComparison.InvariantCultureIgnoreCase);
            if (endIndex < 0) return content;
            endIndex += "</body>".Length;

            return content.Substring(startIndex,endIndex-startIndex);


        }
    }
}
