using MailSanitiserFunction;
using MailSanitiserFunction.Strategies;
using System;
using Xunit;

namespace EmailSentimentAnalysis.Tests.Unit
{
    public class HtmlStrategyTests
    {
        private readonly TestDataHelper _testHelper = new TestDataHelper();
        private string _emailHtmlContent;

        public HtmlStrategyTests()
        {
            _emailHtmlContent = _testHelper.GetFileDataEmbeddedInAssembly("WeeklyEmailHtmlContent.html");
        }
        [Fact]
        public void ShouldStripAllHtml()
        {
            var engine = new MailSanitiserEngine();
            var result = engine.Sanitise(_emailHtmlContent, SanitiseContentType.Html);

            Assert.False(result.Contains('<'));
            Assert.False(result.Contains('>'));
        }

        [Fact]
        public void ShouldStripAllButBodyElementFromBasicHtml()
        {
            var content = "<html><head><title>test data</title></head><body><p>this is my body</p></body></html>";
            var engine = new RemoveAllButBodyStrategy();
            var result = engine.SanitiseContent(content);

            Assert.Equal("<p>this is my body</p>", result);
        }
    }
}
