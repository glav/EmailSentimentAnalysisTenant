﻿using Core;
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
                Dependencies.DiagnosticLogging.Info("{0} mail messages to store.", mailList.Count);

                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameCollectMail);

                foreach (var m in mailList)
                {
                    var op = TableOperation.Insert(m);
                    var result = await tblRef.ExecuteAsync(op);
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("Unable to write MailMessage to table storage {0}", m);
                    }
                }
                Dependencies.DiagnosticLogging.Info("{0} mail messages stored.", mailList.Count);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "Error sending mail list to queue ");
            }
        }

        public Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig, int maxCount = 20)
        {
            return Task.Run<List<RawMailMessageEntity>>(() =>
            {
                var emails = new List<RawMailMessageEntity>();
                Dependencies.DiagnosticLogging.Info("Attempting to collect a maximum of {0} emails", maxCount);

                try
                {
                    using (var emailClient = new Pop3Client())
                    {
                        var emailServer = $"[{emailConfig.PopServerHost}:{emailConfig.PopServerPort}]";
                        Dependencies.DiagnosticLogging.Verbose("Collecting mail from Host:{0}", emailServer);

                        emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        emailClient.Connect(emailConfig.PopServerHost, emailConfig.PopServerPort, SecureSocketOptions.Auto);

                        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                        Dependencies.DiagnosticLogging.Info($"Authenticating to email server {emailServer}, : Username: [{emailConfig.Username}]");

                        emailClient.Authenticate(emailConfig.Username, emailConfig.Password);
                        Dependencies.DiagnosticLogging.Info($"Successfully authenticated to email server:{emailServer}");

                        for (int i = 0; i < emailClient.Count && i < maxCount; i++)
                        {
                            var message = emailClient.GetMessage(i);
                            var emailMessage = new RawMailMessageEntity
                            {
                                Body = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                                Subject = message.Subject
                            };
                            emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new RawEmailAddress { Address = x.Address, Name = x.Name }));
                            emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new RawEmailAddress { Address = x.Address, Name = x.Name }));
                        }

                        Dependencies.DiagnosticLogging.Info("Collected {0} emails from server.", emails.Count);

                        return emails;
                    }
                }
                catch (Exception ex)
                {
                    Dependencies.DiagnosticLogging.Fatal(ex, "Error attempting to collect mail");
                    return emails;
                }
            });
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
