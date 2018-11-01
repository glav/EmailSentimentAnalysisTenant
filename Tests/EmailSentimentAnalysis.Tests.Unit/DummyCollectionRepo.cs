using Core.Data;
using MailCollectorFunction.Config;
using MailCollectorFunction.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class DummyCollectionRepo : IMailCollectionRepository
    {
        private TestFlag _testFlag;
        
        public DummyCollectionRepo(TestFlag flag)
        {
            _testFlag = flag;
        }
        public Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig, int maxCount = 10)
        {
            CollectMailCount++;
            if (_testFlag == TestFlag.BlowUpOnCollection)
            {
                throw new Exception("Big bada boom!");
            }
            return Task.FromResult<List<RawMailMessageEntity>>(null);
        }

        public int LodgeAcknowledgementCount { get; private set; }
        public int CollectMailCount { get; private set; }
        public int StoreMailCount { get; private set; }

        public Task LodgeMailCollectedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            LodgeAcknowledgementCount++;
            return Task.FromResult(0);
        }

        public Task StoreMailAsync(List<RawMailMessageEntity> mailList)
        {
            StoreMailCount++;
            if (_testFlag == TestFlag.BlowUpOnStoring)
            {
                throw new Exception("Big bada boom!");
            }
            return Task.FromResult(0);
        }
    }

    public enum TestFlag
    {
        None,
        BlowUpOnCollection,
        BlowUpOnStoring
    }

}
