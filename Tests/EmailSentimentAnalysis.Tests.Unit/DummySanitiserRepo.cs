using Core.Data;
using MailSanitiserFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSentimentAnalysis.Tests.Unit
{
    class DummySanitiserRepo : IMailSanitiserRepository
    {
        private int _getCollectionCount;
        private int _lodgeMailAckCount;
        private int _storeSanitisedMailCount;
        private GenericActionMessage _actionMessageToLodge;
        private List<SanitisedMailMessageEntity> _dummyMail = new List<SanitisedMailMessageEntity>();

        public DummySanitiserRepo(int numMailMessagesToReturn)
        {
            if (numMailMessagesToReturn > 0)
            {
                for (var cnt=0; cnt < numMailMessagesToReturn; cnt++)
                {
                    _dummyMail.Add(new SanitisedMailMessageEntity { Subject = $"Unit test#{cnt}", Body = $"This is a body #{cnt}" });
                }
            }
        }

        public int MailCollectionCount => _getCollectionCount;
        public int LodgeMailAcknowledgementCount => _lodgeMailAckCount;
        public int StoreSanitisedMaiLCount => _storeSanitisedMailCount;
        public GenericActionMessage ActionMessageRecentlyLodged => _actionMessageToLodge;

        public Task<List<SanitisedMailMessageEntity>> GetCollectedMailAsync()
        {
            _getCollectionCount++;

            return Task.FromResult(_dummyMail);
        }

        public Task LodgeMailSanitisedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            _actionMessageToLodge = receivedMessage;
            _lodgeMailAckCount++;
            return Task.FromResult(0);
        }

        public Task StoreSanitisedMailAsync(List<SanitisedMailMessageEntity> mail)
        {
            _storeSanitisedMailCount++;
            return Task.FromResult(0);
        }
    }
}
