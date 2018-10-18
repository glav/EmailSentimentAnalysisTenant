using MailSanitiserFunction;
using System;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class HtmlStrategyTests
    {
        private readonly TestDataHelper _testHelper = new TestDataHelper();

        [Fact]
        public void ShouldStripAllHtml()
        {
            var engine = new MailSanitiserEngine();
            var emailHtmlContent = _testHelper.GetFileDataEmbeddedInAssembly("WeeklyEmailHtmlContent.html");
            var result = engine.Sanitise(emailHtmlContent, SanitiseContentType.Html);

            Assert.False(result.Contains('<'));
            Assert.False(result.Contains('>'));
        }
    }
}
