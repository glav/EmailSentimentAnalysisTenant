using Core.Data;
using MailCollectorFunction.Config;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MailCollectorFunction.Data
{
    public interface IMailCollectionRepository
    {
        Task StoreMailAsync(List<RawMailMessageEntity> mailList);
        Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig, int maxCount = 10);

        Task LodgeMailCollectedAcknowledgementAsync(GenericActionMessage receivedMessage);
    }
}
