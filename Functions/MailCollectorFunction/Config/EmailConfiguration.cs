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
    }
}
