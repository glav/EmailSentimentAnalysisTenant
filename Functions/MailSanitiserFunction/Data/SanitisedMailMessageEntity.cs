using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSanitiserFunction.Data
{
    public class SanitisedMailMessageEntity : TableEntity
    {
        public SanitisedMailMessageEntity() : base()
        {
        }
        public string FromAddresses { get; set; }
        public string ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SanitisedBody { get; set; }

    }

}
