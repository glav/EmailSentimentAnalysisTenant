using Core.Data;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSchedulerFunction.Data
{
    class MailSchedulerEntity : TableEntity
    {
        public MailSchedulerEntity()
        {
            PartitionKey = DataStores.Tables.SchedulerTablePartitionKey;
            RowKey = DataStores.Tables.SchedulerTableRowKey;
            TimeOfActionUtc = DateTime.UtcNow;
            ETag = "*";
        }
        public DateTime TimeOfActionUtc { get; set; }
    }
}
