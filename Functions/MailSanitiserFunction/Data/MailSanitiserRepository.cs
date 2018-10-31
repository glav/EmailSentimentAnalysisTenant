using Core;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSanitiserFunction.Data
{
    class MailSanitiserRepository : BaseCloudStorageRepository, IMailSanitiserRepository
    {
        public MailSanitiserRepository(CoreDependencyInstances dependencies) : base(dependencies)
        {
        }
    }
}
