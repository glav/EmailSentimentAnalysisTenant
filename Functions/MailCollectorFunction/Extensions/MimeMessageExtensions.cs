using MailCollectorFunction.Data;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Extensions;

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
            if (mimeMsg.To.Count > 0)
            {
                var toList = mimeMsg.To.Select(x => (MailboxAddress)x).Select(x => string.IsNullOrWhiteSpace(x.Name) ? x.Address : $"{x.Address}<{x.Name}>").ToArray();
                msgEntity.ToAddresses = string.Join(";", toList);
            }
               
            if (mimeMsg.From.Count > 0)
            {
                var fromList = mimeMsg.From.Select(x => (MailboxAddress)x).Select(x => string.IsNullOrWhiteSpace(x.Name) ? x.Address : $"{x.Address}<{x.Name}>");
                msgEntity.FromAddresses = string.Join(";", fromList);
            }
            msgEntity.RowKey = (long.MaxValue - DateTime.UtcNow.Ticks).ToString().PadLeft(20, '0');
            var partitionKeyValue = 30000000 - (DateTime.UtcNow.Year * 10000 + (DateTime.UtcNow.Month * 100) + DateTime.UtcNow.Day);
            msgEntity.PartitionKey = partitionKeyValue.ToString().PadLeft(10, '0');  // for sorting

            return msgEntity;
        }
    }
}
