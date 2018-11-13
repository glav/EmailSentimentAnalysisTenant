using Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailProcessorFunction.Data
{
    public interface IMailProcessorRepository
    {
        Task LodgeMailProcessorAcknowledgementAsync(GenericActionMessage receivedMessage);
        Task<List<AnalysedMailMessageEntity>> GetSanitisedMailAsync();
    }
}
