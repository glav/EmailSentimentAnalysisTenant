using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMailApiFunction.Domain
{
    public class QueryApiMessage
    {
        public string FromAddresses { get; set; }
        public string ToAddresses { get; set; }
        public string PrimaryFromAddress { get; set; }
        public string Subject { get; set; }
        public string OrignalBody { get; set; }
        public string SanitisedBody { get; set; }
        public double SentimentClassification { get; set; }
        public string SentimentKeyPhrases { get; set; }
        public DateTime AnalysedTimestampUtc { get; set; }
    }
}
