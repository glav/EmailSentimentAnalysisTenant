using MailCollectorFunction.Data;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailCollectorFunction.Extensions
{
    public static class MimeMessageExtensions
    {
        public static RawMailMessageEntity ToMailMessageEntity(this MimeMessage mimeMsg)
        {
            var msgEntity = new RawMailMessageEntity
            {
                Body = !string.IsNullOrEmpty(mimeMsg.HtmlBody) ? mimeMsg.HtmlBody : mimeMsg.TextBody,
                Subject = mimeMsg.Subject
            };
            msgEntity.ToAddresses.AddRange(mimeMsg.To.Select(x => (MailboxAddress)x).Select(x => new RawEmailAddress { Address = x.Address, Name = x.Name }));
            msgEntity.FromAddresses.AddRange(mimeMsg.From.Select(x => (MailboxAddress)x).Select(x => new RawEmailAddress { Address = x.Address, Name = x.Name }));
            msgEntity.RowKey = msgEntity.ToAddresses.First().Address;
            return msgEntity;
        }
    }
}
