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
        public Task LodgeMailSanitisedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            throw new NotImplementedException();
        }
    }
}
