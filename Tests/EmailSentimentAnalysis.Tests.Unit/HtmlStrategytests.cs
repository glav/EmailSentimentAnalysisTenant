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
        private string _simpleHtmlEmailContent;
        private string _complexHtmlEmailContent;
        private CoreDependencyInstances _coreDependencies;

        const string _textContainsHtmlComment = "<!--/* Font Definitions */@font-face	{font-family:\"Cambria Math\";	panose-1:2 4 5 3 5 4 6 3 2 4;}" +
            "@font-face	{font-family:Calibri;	panose-1:2 15 5 2 2 2 4 3 2 4;}/* Style Definitions */p.MsoNormal, li.MsoNormal, div.MsoNormal	{margin:0cm;	" +
            "margin-bottom:.0001pt;	font-size:11.0pt;	font-family:\"Calibri\",sans-serif;	mso-fareast-language:EN-US;}a:link, span.MsoHyperlink	" +
            "{mso-style-priority:99;	color:#0563C1;	text-decoration:underline;}a:visited, span.MsoHyperlinkFollowed	{mso-style-priority:99;	color:#954F72;	" +
            "text-decoration:underline;}span.EmailStyle17	{mso-style-type:personal-compose;	font-family:\"Calibri\",sans-serif;}.MsoChpDefault	" +
            "{mso-style-type:export-only;	font-family:\"Calibri\",sans-serif;	mso-fareast-language:EN-US;}@page WordSection1	{size:612.0pt 792.0pt;	" +
            "margin:72.0pt 72.0pt 72.0pt 72.0pt;}div.WordSection1	{page:WordSection1;}-->This is yet another test";

        public HtmlStrategyTests()
        {
            _coreDependencies = CoreDependencies.Setup();
            _emailHtmlContent = _testHelper.GetFileDataEmbeddedInAssembly("WeeklyEmailHtmlContent.html");
            _simpleHtmlEmailContent = _testHelper.GetFileDataEmbeddedInAssembly("SImpleEmailInHtml.html");
            _complexHtmlEmailContent = _testHelper.GetFileDataEmbeddedInAssembly("ComplexEmail.html");
        }
        [Fact]
        public void ShouldStripAllHtml()
        {
            var repo = new DummySanitiserRepo(1);
            var engine = new MailSanitiserEngine(_coreDependencies,repo);
            var result = engine.SanitiseContent(_emailHtmlContent, SanitiseContentType.Html);

            Assert.False(result.Contains('<'));
            Assert.False(result.Contains('>'));
        }

        [Fact]
        public void ShouldStripHtmlCommentsFromSimpleContent()
        {
            var repo = new DummySanitiserRepo(1);
            var engine = new MailSanitiserEngine(_coreDependencies, repo);
            var result = engine.SanitiseContent(_simpleHtmlEmailContent, SanitiseContentType.Html & SanitiseContentType.PlainText);

            Assert.DoesNotContain("<!--",result);
            Assert.DoesNotContain("-->", result);
        }

        [Fact]
        public void ShouldStripHtmlCommentsFromComplexContent()
        {
            var repo = new DummySanitiserRepo(1);
            var engine = new MailSanitiserEngine(_coreDependencies, repo);
            var result = engine.SanitiseContent(_complexHtmlEmailContent, SanitiseContentType.Html & SanitiseContentType.PlainText);

            Assert.DoesNotContain("<!--", result);
            Assert.DoesNotContain("-->", result);
        }

        [Fact]
        public void ShouldStripAllButBodyElementFromBasicHtml()
        {
            var content = "<html><head><title>test data</title></head><body><p>this is my body</p></body></html>";
            var engine = new RemoveAllButBodyStrategy();
            var result = engine.SanitiseContent(content);

            Assert.Equal("<body><p>this is my body</p></body>", result);
        }

        [Fact]
        public void ShouldStripAllButBodyElementFromEmailHtml()
        {
            var engine = new RemoveAllButBodyStrategy();
            var result = engine.SanitiseContent(_emailHtmlContent);

            Assert.True(result.IndexOf("<body") == 0);
        }

        [Fact]
        public void ShouldStripAllButBodyElementFromEmailHtmlAndRemoveMarkup()
        {
            var repo = new DummySanitiserRepo(1);
            var engine = new MailSanitiserEngine(_coreDependencies, repo);
            var result = engine.SanitiseForAllContentTypes(_emailHtmlContent);

            Assert.DoesNotContain("<body", result.ToLowerInvariant());
            Assert.DoesNotContain("</body", result.ToLowerInvariant());
            Assert.False(result.Contains('<'));
            Assert.False(result.Contains('>'));
        }

        [Fact]
        public void ShouldStripAllRemainingEncodedCharsFromString()
        {
            const string content = "* &nbsp;This WeekMehdi worked on production issues and finalize internal training.Giovani " +
                "worked on Trix bug fixes.Next WeekMehdi will work on production issues.Giovani will work on Trix new features.Code " +
                "Freeze Dates: The final VNext for 2018 is 19th or 20th Dec – this means to hit that date, we need to merge by 11th DecThe " +
                "first release in Jan is 9th or 10th Jan in 2019 – this means we need to merge by to DM by 2nd JanING Direct (Banking &amp;" +
                " Finance)ING DirectTechnologiesASP.NET WebForms, C#, WCF, VB, VB .NET, Polymer&nbsp;";
            var engine = new RemoveEncodedCharactersStrategy();
            var result = engine.SanitiseContent(content);

            Assert.DoesNotContain("&nbsp;", result.ToLowerInvariant());
            Assert.DoesNotContain("&amp;", result.ToLowerInvariant());
        }
    }
}
