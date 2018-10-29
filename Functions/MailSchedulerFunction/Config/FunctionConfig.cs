namespace CollectMailScheduler.Config
{
    public static class FunctionConfig
    {
        public const string QueueNameCollectEmail = "collectemail";
        public const string QueueNameProcessEmail = "processemail";
        public const string TableNameMailSchedulerStatus = "EmailSentimentMailSchedulerStatus";
        public const string TablePartitionKey = "MailSchedulerPartitionKey";
        public const string TableRowKey = "MailScheduler";
    }

}