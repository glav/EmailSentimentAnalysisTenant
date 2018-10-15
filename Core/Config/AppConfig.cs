using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Config
{
    public class AppConfig : IAppConfig
    {
        public bool IsHostedInAzure
        {
            get
            {
                return !string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME", EnvironmentVariableTarget.Process));
            }
        }
    }
}
