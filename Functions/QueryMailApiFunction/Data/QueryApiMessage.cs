using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMailApiFunction.Data
{
    public class QueryApiMessage
    {
        public List<RawEmailAddress> FromAddresses { get; set; }
        public List<RawEmailAddress> ToAddresses { get; set; }
        public string PrimaryFromAddress { get; set; }
        public string Subject { get; set; }
        public string OrignalBody { get; set; }
        public string SanitisedBody { get; set; }
        public double SentimentClassification { get; set; }
        public string SentimentKeyPhrases { get; set; }
        public DateTime AnalysedTimestampUtc { get; set; }
    }
}
