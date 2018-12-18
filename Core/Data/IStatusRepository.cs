using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface IStatusRepository
    {
        Task UpdateStatusAsync(string message);
        Task ClearStatusAsync();
        Task<StatusUpdateEntity> GetStatusAsync();
    }
}
