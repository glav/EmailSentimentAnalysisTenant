using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data
{
    public class GenericActionMessage
    {
        public GenericActionMessage()
        {
            ActionDateTimeUtc = DateTime.UtcNow;
            CorrelationId = Guid.NewGuid();
        }
        public DateTime ActionDateTimeUtc { get; set; }
        public Guid CorrelationId { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static GenericActionMessage FromString(string objectData)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<GenericActionMessage>(objectData);
            } catch
            {
                return null;
            }
        }

        public static CloudQueueMessage CreateNewQueueMessage()
        {
            return new CloudQueueMessage(new GenericActionMessage().ToString());
        }

        public static CloudQueueMessage CreateQueueMessageFromExistingMessage(GenericActionMessage msg)
        {
            return CreateQueueMessageFromCorrelationId(msg.CorrelationId);
        }
        public static CloudQueueMessage CreateQueueMessageFromCorrelationId(Guid correlationid)
        {
            var msg = new GenericActionMessage();
            msg.CorrelationId = correlationid;
            return new CloudQueueMessage(msg.ToString());
        }
    }
}
