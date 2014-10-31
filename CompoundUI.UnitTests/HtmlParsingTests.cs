using System;
using FluentAssertions;
using HtmlAgilityPack;
using NSubstitute;
using Xunit;

namespace CompoundUI.UnitTests
{
    public class HtmlParsingTests
    {
        public class TheParseMethod
        {
            [Fact]
            public void ShouldReplaceATransclusionElementsWithDivs()
            {
                // Arrange
                var htmlText = "<html><body><trns src='http://test.com'></trns></body></html>";
                var htmlParser = GetHtmlParser();

                // Act
                htmlParser.Parse(htmlText);

                // Assert
                htmlParser.GetParsedHtml().Should().Contain("div");
            }

            [Fact]
            public void ShouldReplaceAllTransclusionElementsWithDivs()
            {
                // Arrange
                var htmlWith3ReplacableSections = "<html><body><trns src='http://test.com'></trns><p>Test</p><trns src='http://test.com'></trns><trns src='http://test.com'></trns></body></html>";
                var htmlParser = GetHtmlParser();

                // Act
                htmlParser.Parse(htmlWith3ReplacableSections);

                // Assert
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlParser.GetParsedHtml());
                var embeddedDivs = htmlDoc.DocumentNode.SelectNodes("//div");
                embeddedDivs.Should().HaveCount(3);
            }

            [Fact]
            public void ShouldReplaceAllTransclusionElementsContentFromSpecifiedUrlSrc()
            {
                // Arrange
                var htmlToEmbed = "<p>Embedded html from source</p>";
                var sourceResolver = Substitute.For<IResolveHtmlSources>();
                sourceResolver.Resolve("http://test.com").Returns(htmlToEmbed);
                var htmlText = "<html><body><trns src='http://test.com'></trns></body></html>";
                var htmlParser = GetHtmlParser(sourceResolver);

                // Act
                htmlParser.Parse(htmlText);

                // Assert
                htmlParser.GetParsedHtml().Should().Contain(htmlToEmbed);
            }

            private HtmlParser GetHtmlParser(IResolveHtmlSources resolveHtmlSources = null)
            {
                return new HtmlParser(resolveHtmlSources ?? Substitute.For<IResolveHtmlSources>());
            }
        }
    }

    public interface IResolveHtmlSources
    {
        string Resolve(string source);
    }

    public class HtmlParser
    {
        private readonly IResolveHtmlSources _htmlSourcesResolver;
        private HtmlDocument _htmlDocument;

        public HtmlParser(IResolveHtmlSources htmlSourcesResolver)
        {
            _htmlSourcesResolver = htmlSourcesResolver;
        }

        public void Parse(string htmlText)
        {
            _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(htmlText);
            var transcludorNodes = _htmlDocument.DocumentNode.SelectNodes("//trns");
            foreach (var transcludorNode in transcludorNodes)
            {
                var htmlSource = transcludorNode.Attributes["src"].Value;
                var html = _htmlSourcesResolver.Resolve(htmlSource);
                var newNode = HtmlNode.CreateNode(String.Format("<div>{0}</div>", html));
                transcludorNode.ParentNode.ReplaceChild(newNode, transcludorNode);
            }
        }

        public string GetParsedHtml()
        {
            return _htmlDocument.DocumentNode.OuterHtml;
        }

    }
}