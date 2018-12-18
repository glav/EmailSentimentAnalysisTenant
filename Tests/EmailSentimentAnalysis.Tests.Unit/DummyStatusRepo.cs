using Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class DummyStatusRepo : IStatusRepository
    {
        public Task ClearStatusAsync()
        {
            return Task.FromResult(0);
        }

        public Task<StatusUpdateEntity> GetStatusAsync()
        {
            return Task.FromResult<StatusUpdateEntity>(new StatusUpdateEntity { Message = "In unit test" });
        }

        public Task UpdateStatusAsync(string message)
        {
            return Task.FromResult(0);
        }
    }
}
