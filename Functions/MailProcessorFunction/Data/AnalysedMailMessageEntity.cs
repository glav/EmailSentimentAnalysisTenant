using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailProcessorFunction.Data
{
    public class AnalysedMailMessageEntity : TableEntity
    {
        public AnalysedMailMessageEntity() : base()
        {
            AnalysedTimestampUtc = DateTime.UtcNow;
        }
        public string FromAddresses { get; set; }
        public string ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SanitisedBody { get; set; }
        public double SentimentClassification { get; set; }
        public string SentimentKeyPhrases { get; set; }
        public DateTime AnalysedTimestampUtc { get; set; }

    }
}
