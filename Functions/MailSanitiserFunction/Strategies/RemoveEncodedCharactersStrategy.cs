using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MailSanitiserFunction.Strategies
{
    public class RemoveEncodedCharactersStrategy : IMailSanitiserStrategy
    {
        /*
         * &nbsp;This WeekMehdi worked on production issues and finalize internal training.Giovani worked on Trix bug fixes.Next WeekMehdi will work on production issues.Giovani will work on Trix new features.Code Freeze Dates: The final VNext for 2018 is 19th or 20th Dec – this means to hit that date, we need to merge by 11th DecThe first release in Jan is 9th or 10th Jan in 2019 – this means we need to merge by to DM by 2nd JanING Direct (Banking &amp; Finance)ING DirectTechnologiesASP.NET WebForms, C#, WCF, VB, VB .NET, Polymer&nbsp;Powered by ReadiUpdates (change your subscription settings)
         */
        private static List<EncodedCharReplacement> EncodedCharacters = new List<EncodedCharReplacement>
        {
            new EncodedCharReplacement("&nbsp;"," "),
            new EncodedCharReplacement("&amp;","&")
        };

        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }
            var builder = new StringBuilder(content);
            EncodedCharacters.ForEach(e =>
            {
                builder.Replace(e.EncodedString, e.ReplacementString);
            });
            return builder.ToString();
        }

        private class EncodedCharReplacement
        {
            public EncodedCharReplacement(string encoded, string replacement)
            {
                EncodedString = encoded;
                ReplacementString = replacement;
            }
            public string EncodedString { get; set; }
            public string ReplacementString { get; set; }
        }
    }

}
