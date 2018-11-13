using Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailSanitiserFunction.Data
{
    public interface IMailSanitiserRepository
    {
        Task LodgeMailSanitisedAcknowledgementAsync(GenericActionMessage receivedMessage);
        Task<List<SanitisedMailMessageEntity>> GetCollectedMailAsync();
        Task ClearCollectedMailAsync();
        Task StoreSanitisedMailAsync(List<SanitisedMailMessageEntity> mail);
    }
}
