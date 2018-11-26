using System.Collections.Generic;
using System.Linq;

namespace QueryMailApiFunction.Data
{
    public static class DataExtensions
    {
        public static QueryApiMessage ToApiMessage(this MailSentimentMessageEntity entity)
        {
            var apiMessage = new QueryApiMessage();
            if (entity == null)
            {
                return apiMessage;
            }
            apiMessage.AnalysedTimestampUtc = entity.AnalysedTimestampUtc;
            apiMessage.PrimaryFromAddress = entity.PrimaryFromAddress;
            apiMessage.OrignalBody = entity.Body;
            apiMessage.FromAddresses = entity.FromAddresses;
            apiMessage.SanitisedBody = entity.SanitisedBody;
            apiMessage.SentimentClassification = entity.SentimentClassification;
            apiMessage.SentimentKeyPhrases = entity.SentimentKeyPhrases;
            apiMessage.Subject = entity.Subject;
            apiMessage.ToAddresses = entity.ToAddresses;
            return apiMessage;
        }

        public static IEnumerable<QueryApiMessage> ToApiMessages(this IEnumerable<MailSentimentMessageEntity> entities)
        {
            var apiMessages = new List<QueryApiMessage>();
            if (entities == null || entities.Count() == 0)
            {
                return apiMessages;
            }
            entities.ToList().ForEach(e =>
            {
                apiMessages.Add(e.ToApiMessage());
            });
            return apiMessages;
        }
    }
}
