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
            FromAddresses = new List<RawEmailAddress>();
            ToAddresses = new List<RawEmailAddress>();
        }
        public List<RawEmailAddress> FromAddresses { get; set; }
        public List<RawEmailAddress> ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SanitisedBody { get; set; }

    }

    public class RawEmailAddress
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
