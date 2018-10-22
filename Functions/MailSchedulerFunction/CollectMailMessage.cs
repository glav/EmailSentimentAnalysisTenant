using System;

namespace CollectMailScheduler
{
    public class CollectMailMessage
    {
        public CollectMailMessage()
        {
            DateTimeOriginated = DateTime.UtcNow;
            CorrelationId = Guid.NewGuid();
        }
        public DateTime DateTimeOriginated {get; set;}
        public Guid CorrelationId {get; set;}

    }
}