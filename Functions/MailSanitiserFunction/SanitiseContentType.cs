using System;
using System.Collections.Generic;
using System.Text;

namespace MailSanitiserFunction
{
    [Flags]
    public enum SanitiseContentType : Byte
    {
        PlainText,
        Html
    }
}
