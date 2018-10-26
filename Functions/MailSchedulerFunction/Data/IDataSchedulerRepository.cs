using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailSchedulerFunction.Data
{
    public interface IDataSchedulerRepository
    {
        Task<bool> IsMailOperationInProgressAsync();
        Task SetMailOperationToInProgressAsync();
        Task ClearMailOperationProgressAsync();
    }
}
