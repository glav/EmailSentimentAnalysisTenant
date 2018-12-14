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
                Dependencies.DiagnosticLogging.Verbose("MailCollection: No email to store, exiting.");
                return;
            }
            var numMsgs = mailList.Count;
            int storedMsgs = 0;
            Dependencies.DiagnosticLogging.Verbose("MailCollection: {numMsgs} mail messages to store.", numMsgs);
            var tblRef = CreateClientTableReference(DataStores.Tables.TableNameCollectMail);


            foreach (var mail in mailList)
            {
                try
                {
                    var op = TableOperation.Insert(mail);
                    var result = await tblRef.ExecuteAsync(op);  // <-- error happens here
                    if (result.HttpStatusCode >= 300)
                    {
                        Dependencies.DiagnosticLogging.Error("MailCollection: Unable to write MailMessage to table storage {m}", mail);
                    }
                    storedMsgs++;
                }
                catch (Microsoft.WindowsAzure.Storage.StorageException sx)
                {
                    Dependencies.DiagnosticLogging.Fatal(sx, "MailCollection: Error sending mail list to queue - StorageIssue, {@ExtendedErrorInformation} [{@m}]", sx.RequestInformation.ExtendedErrorInformation, mail);
                }
                catch (Exception ex)
                {
                    Dependencies.DiagnosticLogging.Fatal(ex, "MailCollection: Error sending mail list to queue [{@m}]",mail);
                }
            }
            Dependencies.DiagnosticLogging.Verbose("MailCollection: Mail messages stored: #{storedMsgs}", storedMsgs);
        }

        public async Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig)
        {
            var emailsRetrieved = new List<RawMailMessageEntity>();
            var emailsOnServer = new List<MimeMessage>();
            Dependencies.DiagnosticLogging.Verbose("MailCollection: Attempting to collect a maximum of {maxEmails} emails", emailConfig.MaxEmailsToRetrieve);

            try
            {
                using (var emailClient = new Pop3Client())
                {
                    var emailServerInfo = $"[{emailConfig.PopServerHost}:{emailConfig.PopServerPort}]";
                    Dependencies.DiagnosticLogging.Verbose("MailCollection: Collecting mail from Host:{emailServer}", emailServerInfo);

                    await SetupConnectToEmailServerAndAuthenticate(emailConfig, emailClient, emailServerInfo);

                    var msgCountToCollect = emailClient.Count > emailConfig.MaxEmailsToRetrieve ? emailConfig.MaxEmailsToRetrieve : emailClient.Count; ;
                    Dependencies.DiagnosticLogging.Debug("MailCollection: Successfully authenticated to email server:{emailServer}, {msgCount} mail msgs in queue, retrieving {msgCountToCollect}",
                        emailServerInfo, emailClient.Count, msgCountToCollect);
                    if (msgCountToCollect == 0)
                    {
                        return emailsRetrieved;
                    }
                    emailsOnServer.AddRange(await emailClient.GetMessagesAsync(0, msgCountToCollect));
                    await DeleteMessagesIfRequired(emailConfig, emailClient, msgCountToCollect);
                    await emailClient.DisconnectAsync(true);

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
            }
            catch (Exception ex)
            {
                Dependencies.DiagnosticLogging.Fatal(ex, "MailCollection: Error deleting collected messages");
            }
        }

        private async Task SetupConnectToEmailServerAndAuthenticate(EmailConfiguration emailConfig, Pop3Client emailClient, string emailServerInfo)
        {
            emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await emailClient.ConnectAsync(emailConfig.PopServerHost, emailConfig.PopServerPort, SecureSocketOptions.Auto);
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            Dependencies.DiagnosticLogging.Verbose("MailCollection: Authenticating to email server {emailServerInfo}, : Username: [{Username}]", emailServerInfo, emailConfig.Username);
            await emailClient.AuthenticateAsync(emailConfig.Username, emailConfig.Password);
        }

        public async Task LodgeMailCollectedAcknowledgementAsync(GenericActionMessage receivedMessage)
        {
            await LodgeAcknowledgementMessageAsync(receivedMessage, "MailCollection", DataStores.Queues.QueueNameCleanEmail);
        }

    }
}
