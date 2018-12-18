using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data
{
    public class StatusUpdateEntity : TableEntity
    {
        public StatusUpdateEntity()
        {
            PartitionKey = DataStores.Tables.StatusTablePartitionKey;
            RowKey = DataStores.Tables.StatusTableRowKey;
        }
        public string Message { get; set; }
    }
}
