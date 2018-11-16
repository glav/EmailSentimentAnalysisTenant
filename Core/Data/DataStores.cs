using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data
{
    public static class DataStores
    {
        public static class Queues
        {
            public const string QueueNameCollectEmail = "collectemail";
            public const string QueueNameCleanEmail = "cleanemail";
            public const string QueueNameProcessEmail = "processemail";
            public const string QueueNameTriggerEmail = "triggeremail";
        }

        public static class Tables
        {
            public const string TableNameCollectMail = "EmailSentimentCollectMail";
            public const string TableNameSanitisedMail = "EmailSentimentSanitisedMail";
            public const string TableNameMailSchedulerStatus = "EmailSentimentMailSchedulerStatus";
            public const string TableNameProcessed = "EmailSentimentMailProcessed";

            // Please ensure that any table names added as constants above are also added to this list 
            // to ensure they get created by infra process.
            public static readonly string[] FullTableList = new string[] 
            {
                TableNameCollectMail,
                TableNameMailSchedulerStatus,
                TableNameSanitisedMail,
                TableNameProcessed
            }; 

            public const string SchedulerTablePartitionKey = "MailSchedulerPartitionKey";
            public const string SchedulerTableRowKey = "MailScheduler";

        }
    }
}
