using System;
using System.Collections.Generic;
using System.Text;

namespace MailSchedulerFunction.Data
{
    public interface IDataSchedulerRepository
    {
        bool IsMailOperationInProgress();
        void SetMailOperationToInProgress();
        void ClearMailOperationProgress();
    }
}
