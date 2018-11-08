using Core;
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
        private CoreDependencyInstances _coreDependencies;

        public HtmlStrategyTests()
        {
            _coreDependencies = CoreDependencies.Setup();
            _emailHtmlContent = _testHelper.GetFileDataEmbeddedInAssembly("WeeklyEmailHtmlContent.html");
        }
        [Fact]
        public void ShouldStripAllHtml()
        {
            var repo = new DummySanitiserRepo();
            var engine = new MailSanitiserEngine(_coreDependencies,repo);
            var result = engine.SanitiseContent(_emailHtmlContent, SanitiseContentType.Html);

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

        [Fact]
        public void ShouldStripAllButBodyElementFromEmailHtml()
        {
            var engine = new RemoveAllButBodyStrategy();
            var result = engine.SanitiseContent(_emailHtmlContent);

            Assert.DoesNotContain("<body",result.ToLowerInvariant());
            Assert.DoesNotContain("</body", result.ToLowerInvariant());
        }

        [Fact]
        public void ShouldStripAllButBodyElementFromEmailHtmlAndRemoveMarkup()
        {
            var repo = new DummySanitiserRepo();
            var engine = new MailSanitiserEngine(_coreDependencies, repo);
            var result = engine.SanitiseForAllContentTypes(_emailHtmlContent);

            Assert.DoesNotContain("<body", result.ToLowerInvariant());
            Assert.DoesNotContain("</body", result.ToLowerInvariant());
            Assert.False(result.Contains('<'));
            Assert.False(result.Contains('>'));
        }
    }
}
