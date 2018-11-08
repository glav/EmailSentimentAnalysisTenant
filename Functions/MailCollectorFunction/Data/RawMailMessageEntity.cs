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
            FromAddresses = new List<RawEmailAddress>();
            ToAddresses = new List<RawEmailAddress>();
            PartitionKey = Guid.NewGuid().ToString();
        }
        public List<RawEmailAddress> FromAddresses { get; set; }
        public List<RawEmailAddress> ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }

    public class RawEmailAddress
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }

}
