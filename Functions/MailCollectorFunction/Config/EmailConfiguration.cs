using Core;
using Core.Config;
using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailCollectorFunction.Config
{
    public class EmailConfiguration
    {
        public string PopServerHost { get; set; }
        public int PopServerPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public int MaxEmailsToRetrieve { get; set; }

        public bool DeleteMailFromServerOnceCollected { get; set; }
        public static EmailConfiguration PopulateConfigFromEnviromentVariables(CoreDependencyInstances dependencies)
        {
            var mailConfig = new EmailConfiguration();
            var envReader = new EnvironmentValueReader(dependencies.DiagnosticLogging);


            mailConfig.PopServerHost = 
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailhostname" });
            mailConfig.PopServerPort =
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailport" }, "995").ToInt();
            mailConfig.Username = 
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailusername" });
            mailConfig.Password = 
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailpassword" });
            mailConfig.UseSsl =
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailusessl" }).ToBool();
            mailConfig.DeleteMailFromServerOnceCollected =
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "deletemailfromServeroncecollected" }).ToBool();
            mailConfig.MaxEmailsToRetrieve =
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "maxemailstoretrieve" }, "10").ToInt();
            return mailConfig;
        }
    }
}
