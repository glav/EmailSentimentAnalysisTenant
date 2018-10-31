using CollectMailScheduler.Config;
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
            PartitionKey = FunctionConstants.TablePartitionKey;
            RowKey = FunctionConstants.TableRowKey;
            TimeOfActionUtc = DateTime.UtcNow;
        }
        public DateTime TimeOfActionUtc { get; set; }
    }
}
