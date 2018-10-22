using System;
using System.Collections.Generic;
using System.Text;

namespace MailSanitiserFunction
{
    public interface IMailSanitiserStrategy
    {
        SanitiseContentType ContentTypeSupported { get; }
        string SanitiseContent(string content);
    }
}
