using Core;
using Core.Data;
using MailCollectorFunction.Config;
using MailKit.Net.Pop3;
using MailKit.Security;
using Microsoft.WindowsAzure.Storage.Table;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailCollectorFunction.Data
{
    class MailCollectionRepository : BaseCloudStorageRepository, IMailCollectionRepository
    {
        public MailCollectionRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {
        }
        public async Task StoreMailAsync(List<RawMailMessageEntity> mailList)
        {

            if (mailList == null || mailList.Count == 0)
            {
                Dependencies.DiagnosticLogging.Info("No email to store, exiting.");
                return;
            }
            try
            {
                Dependencies.DiagnosticLogging.Verbose($"{mailList.Count} mail messages to store.");

                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameCollectMail);

                foreach (var m in mailList)
                {
                    m.PartitionKey = Guid.NewGuid().ToString();
                    m.RowKey = m.ToAddresses.First().Address;
                    var op = TableOperation.Insert(m);
                    var result = await tblRef.ExecuteAsync(op);
                    Dependencies.DiagnosticLogging.Info("Done Writing message");
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("Unable to write MailMessage to table storage {m}", m);
                    }
                }
                Dependencies.DiagnosticLogging.Info($"{mailList.Count} mail messages stored.");
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "Error sending mail list to queue ");
                var baseEx = ex.GetBaseException();
                if (baseEx != null)
                {
                    Dependencies.DiagnosticLogging.Error(baseEx, "Error sending mail list to queue (Inner/base error)");
                }
            }
        }

        public async Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig, int maxCount = EmailConfiguration.MaxEmailToRetrievePerCall)
        {
            var emails = new List<RawMailMessageEntity>();
            Dependencies.DiagnosticLogging.Info("Attempting to collect a maximum of {maxCount} emails", maxCount);

            try
            {
                using (var emailClient = new Pop3Client())
                {
                    var emailServer = $"[{emailConfig.PopServerHost}:{emailConfig.PopServerPort}]";
                    Dependencies.DiagnosticLogging.Verbose("Collecting mail from Host:{emailServer}", emailServer);

                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await emailClient.ConnectAsync(emailConfig.PopServerHost, emailConfig.PopServerPort, SecureSocketOptions.Auto);

                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    Dependencies.DiagnosticLogging.Info($"Authenticating to email server {emailServer}, : Username: [{emailConfig.Username}]");

                    await emailClient.AuthenticateAsync(emailConfig.Username, emailConfig.Password);
                    var msgCount = emailClient.Count;

                    Dependencies.DiagnosticLogging.Info($"Successfully authenticated to email server:{emailServer}, {msgCount} mail msgs in queue");

                    for (int i = 0; i < msgCount && i < maxCount; i++)
                    {
                        var message = await emailClient.GetMessageAsync(i);
                        var emailMessage = new RawMailMessageEntity
                        {
                            Body = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                            Subject = message.Subject
                        };
                        emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new RawEmailAddress { Address = x.Address, Name = x.Name }));
                        emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new RawEmailAddress { Address = x.Address, Name = x.Name }));
                        emails.Add(emailMessage);
                    }

                    await emailClient.DisconnectAsync(true);
                    Dependencies.DiagnosticLogging.Info($"Collected {emails.Count} emails from server.");

                    return emails;
                }
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Fatal(ex, "Error attempting to collect mail");
                return emails;
            }
        }

        public async Task LodgeMailCollectedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            Dependencies.DiagnosticLogging.Info("Lodging Mail Collected Acknowledgement");
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(DataStores.Queues.QueueNameCleanEmail);
            var msg = receivedMessage == null ? GenericActionMessage.CreateNewQueueMessage() : GenericActionMessage.CreateQueueMessageFromExistingMessage(receivedMessage);
            await queueRef.AddMessageAsync(msg);
            Dependencies.DiagnosticLogging.Info("Mail Collected Acknowledgement lodged.");
        }

    }
}
