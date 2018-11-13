using Core;
using Core.Data;
using MailCollectorFunction.Config;
using MailCollectorFunction.Extensions;
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
                Dependencies.DiagnosticLogging.Info("MailCollection: No email to store, exiting.");
                return;
            }
            var numMsgs = mailList.Count;
            try
            {
                Dependencies.DiagnosticLogging.Verbose($"MailCollection: {numMsgs} mail messages to store.");
                var tblRef = CreateClientTableReference(DataStores.Tables.TableNameCollectMail);

                foreach (var m in mailList)
                {
                    var op = TableOperation.Insert(m);
                    var result = await tblRef.ExecuteAsync(op);
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("MailCollection: Unable to write MailMessage to table storage {m}", m);
                    }
                }
                Dependencies.DiagnosticLogging.Info("MailCollection: Mail messages stored: #{numMsgs}", numMsgs);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "MailCollection: Error sending mail list to queue ");
                var baseEx = ex.GetBaseException();
                if (baseEx != null)
                {
                    Dependencies.DiagnosticLogging.Error(baseEx, "MailCollection: Error sending mail list to queue (Inner/base error)");
                }
            }
        }

        public async Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig)
        {
            var emailsRetrieved = new List<RawMailMessageEntity>();
            var emailsOnServer = new List<MimeMessage>();
            Dependencies.DiagnosticLogging.Verbose($"MailCollection: Attempting to collect a maximum of {emailConfig.MaxEmailsToRetrieve} emails");

            try
            {
                using (var emailClient = new Pop3Client())
                {
                    var emailServerInfo = $"[{emailConfig.PopServerHost}:{emailConfig.PopServerPort}]";
                    Dependencies.DiagnosticLogging.Verbose("MailCollection: Collecting mail from Host:{emailServer}", emailServerInfo);

                    await SetupConnectToEmailServerAndAuthenticate(emailConfig, emailClient, emailServerInfo);

                    var msgCountToCollect = emailClient.Count > emailConfig.MaxEmailsToRetrieve ? emailConfig.MaxEmailsToRetrieve : emailClient.Count; ;
                    Dependencies.DiagnosticLogging.Info("MailCollection: Successfully authenticated to email server:{emailServer}, {msgCount} mail msgs in queue, retrieving {msgCountToCollect}",
                        emailServerInfo, emailClient.Count, msgCountToCollect);
                    if (msgCountToCollect == 0)
                    {
                        return emailsRetrieved;
                    }
                    emailsOnServer.AddRange(await emailClient.GetMessagesAsync(0, msgCountToCollect));
                    await DeleteMessagesIfRequired(emailConfig, emailClient, msgCountToCollect);

                    var cnt = emailsOnServer.Count;
                    Dependencies.DiagnosticLogging.Info("MailCollection: Collected {cnt} emails from server.", cnt);

                    emailsRetrieved.AddRange(emailsOnServer.Select(m => m.ToMailMessageEntity()));
                    return emailsRetrieved;
                }
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Fatal(ex, "MailCollection: Error attempting to collect mail");
                return emailsRetrieved;
            }
        }

        private async Task DeleteMessagesIfRequired(EmailConfiguration emailConfig, Pop3Client emailClient, int msgCountToCollect)
        {
            try
            {
                if (emailConfig.DeleteMailFromServerOnceCollected)
                {
                    await emailClient.DeleteMessagesAsync(0, msgCountToCollect);
                }
                await emailClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Error(ex, "MailCollection: Error deleting collected messages");
            }
        }

        private async Task SetupConnectToEmailServerAndAuthenticate(EmailConfiguration emailConfig, Pop3Client emailClient, string emailServerInfo)
        {
            emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await emailClient.ConnectAsync(emailConfig.PopServerHost, emailConfig.PopServerPort, SecureSocketOptions.Auto);
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            Dependencies.DiagnosticLogging.Verbose($"MailCollection: Authenticating to email server {emailServerInfo}, : Username: [{emailConfig.Username}]");
            await emailClient.AuthenticateAsync(emailConfig.Username, emailConfig.Password);
        }

        public async Task LodgeMailCollectedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            Dependencies.DiagnosticLogging.Verbose("MailCollection: Lodging Mail Collected Acknowledgement");
            var acct = CreateStorageAccountReference();
            var queueClient = acct.CreateCloudQueueClient();
            var queueRef = queueClient.GetQueueReference(DataStores.Queues.QueueNameCleanEmail);
            var msg = receivedMessage == null ? GenericActionMessage.CreateNewQueueMessage() : GenericActionMessage.CreateQueueMessageFromExistingMessage(receivedMessage);
            await queueRef.AddMessageAsync(msg);
            Dependencies.DiagnosticLogging.Info("MailCollection: Mail Collected Acknowledgement lodged.");
        }

    }
}
