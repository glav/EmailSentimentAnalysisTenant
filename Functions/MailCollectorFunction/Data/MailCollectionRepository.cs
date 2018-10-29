using MailCollectorFunction.Config;
using Core;
using Core.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Pop3;
using System.Linq;
using MimeKit;

namespace MailCollectorFunction.Data
{
    class MailCollectionRepository : IMailCollectionRepository
    {
        private readonly CoreDependencyInstances _dependencies;

        public MailCollectionRepository(CoreDependencyInstances dependencies)
        {
            _dependencies = dependencies;
        }
        public async Task StoreMailAsync(List<RawMailMessageEntity> mailList)
        {

            if (mailList == null || mailList.Count == 0)
            {
                _dependencies.DiagnosticLogging.Info("No email to store, exiting.");
                return;
            }
            try
            {
                _dependencies.DiagnosticLogging.Info("{0} mail messages to store.",mailList.Count);

                var tblRef = CreateClientTableReference();

                foreach (var m in mailList)
                {
                    var op = TableOperation.Insert(m);
                    var result = await tblRef.ExecuteAsync(op);
                    if (result.HttpStatusCode >= 300)
                    {
                        _dependencies.DiagnosticLogging.Error("Unable to write MailMessage to table storage {0}", m);
                    }
                }
                _dependencies.DiagnosticLogging.Info("{0} mail messages stored.", mailList.Count);
            }
            catch (Exception ex)
            {
                _dependencies.DiagnosticLogging.Error(ex, "Error sending mail list to queue ");
            }
        }

        public Task<List<RawMailMessageEntity>> CollectMailAsync(EmailConfiguration emailConfig, int maxCount = 20)
        {
            return Task.Run<List<RawMailMessageEntity>>(() =>
            {
                var emails = new List<RawMailMessageEntity>();
                _dependencies.DiagnosticLogging.Info("Attempting to collect a maximum of {0} emails", maxCount);

                try
                {
                    using (var emailClient = new Pop3Client())
                    {
                        _dependencies.DiagnosticLogging.Verbose("Collecting mail from Host:{0}, Port:{1}", emailConfig.PopServerHost, emailConfig.PopServerPort);
                        emailClient.Connect(emailConfig.PopServerHost, emailConfig.PopServerPort, true);

                        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                        _dependencies.DiagnosticLogging.Info("Authenticating to email server");

                        emailClient.Authenticate(emailConfig.Username, emailConfig.Password);

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

                        _dependencies.DiagnosticLogging.Info("Collected a {0} emails from server.", emails.Count);

                        return emails;
                    }
                } catch (Exception ex)
                {
                    _dependencies.DiagnosticLogging.Fatal(ex, "Error attempting to collect mail");
                    return emails;
                }
            });
        }

        private CloudTable CreateClientTableReference()
        {
            var acct = CreateStorageAccountReference();
            var client = acct.CreateCloudTableClient();
            return client.GetTableReference(FunctionConfig.TableNameCollectMail);

        }

        private CloudStorageAccount CreateStorageAccountReference()
        {
            CloudStorageAccount cloudAcct;
            var connString = _dependencies.EnvironmentValueReader.GetEnvironmentValueThatIsNotEmpty(new string[] { ConfigKeys.StorageConnectionString });

            if (!CloudStorageAccount.TryParse(connString, out cloudAcct))
            {
                _dependencies.DiagnosticLogging.Fatal("Unable to parse connection string: {0}", connString);
                throw new Exception($"Unable to parse connection string: {connString}");
            }

            return cloudAcct;

        }
    }
}
