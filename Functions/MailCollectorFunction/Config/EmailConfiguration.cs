using Core;
using Core.Config;
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

        public static EmailConfiguration PopulateConfigFromEnviromentVariables(CoreDependencyInstances dependencies)
        {
            var mailConfig = new EmailConfiguration();
            var envReader = new EnvironmentValueReader(dependencies.DiagnosticLogging);


            mailConfig.PopServerHost = envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailhostname"}, "pop.gmail.com");
            mailConfig.PopServerPort = Convert.ToInt32(envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailport" }, "995"));
            mailConfig.Username = envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailusername" });
            mailConfig.Password = envReader.GetEnvironmentValueThatIsNotEmpty(new string[] { "popemailpassword" });
            return mailConfig;
        }
    }
}
