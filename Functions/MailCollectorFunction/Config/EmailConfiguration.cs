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
        public const int MaxEmailToRetrievePerCall = 10;

        public string PopServerHost { get; set; }
        public int PopServerPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }

        public static EmailConfiguration PopulateConfigFromEnviromentVariables(CoreDependencyInstances dependencies)
        {
            var mailConfig = new EmailConfiguration();
            var envReader = new EnvironmentValueReader(dependencies.DiagnosticLogging);


            mailConfig.PopServerHost = 
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailhostname"}, "pop.gmail.com");
            mailConfig.PopServerPort =
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailport" }, "995").ToInt();
            mailConfig.Username = 
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailusername" });
            mailConfig.Password = 
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailpassword" });
            mailConfig.UseSsl =
                envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailusessl" }).ToBool();
            return mailConfig;
        }
    }
}
