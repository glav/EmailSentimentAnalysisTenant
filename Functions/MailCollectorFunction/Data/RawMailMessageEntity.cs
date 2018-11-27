using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailCollectorFunction.Data
{
    public class RawMailMessageEntity : TableEntity
    {
        public RawMailMessageEntity() : base()
        {
        }
        public string FromAddresses { get; set; }
        public string ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }


}
