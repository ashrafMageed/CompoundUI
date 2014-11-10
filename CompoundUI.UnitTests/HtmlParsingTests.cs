using CompoundUI.Core;
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
                var htmlText = "<html><body><trns src='http://test1.com'></trns></body></html>";
                var htmlParser = GetHtmlParser();

                // Act
                var parsedHtml = htmlParser.Parse(htmlText);

                // Assert
                parsedHtml.Should().Contain("div");
            }

            [Fact]
            public void ShouldReplaceAllTransclusionElementsWithDivs()
            {
                // Arrange
                var htmlWith3ReplacableSections = "<html><body><trns src='http://test2.com'></trns><p>Test</p><trns src='http://test2.com'></trns><trns src='http://test2.com'></trns></body></html>";
                var htmlParser = GetHtmlParser();

                // Act
                var parsedHtml = htmlParser.Parse(htmlWith3ReplacableSections);

                // Assert
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(parsedHtml);
                var embeddedDivs = htmlDoc.DocumentNode.SelectNodes("//div");
                embeddedDivs.Should().HaveCount(3);
            }

            [Fact]
            public void ShouldReplaceAllTransclusionElementsContentFromSpecifiedUrlSrc()
            {
                // Arrange
                var htmlToEmbed = "<p>Embedded html from source</p>";
                var sourceResolver = Substitute.For<IResolveHtmlSources>();
                sourceResolver.Resolve("http://test3.com").Returns(htmlToEmbed);
                var htmlText = "<html><body><trns src='http://test3.com'></trns><trns src='http://test3.com'></trns></body></html>";
                var htmlParser = GetHtmlParser(sourceResolver);

                // Act
                var parsedHtml = htmlParser.Parse(htmlText);

                // Assert
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(parsedHtml);
                var embeddedDivs = htmlDoc.DocumentNode.SelectNodes("//div");
                embeddedDivs.Should().HaveCount(2);
                embeddedDivs[0].InnerHtml.Should().Be(htmlToEmbed);
                embeddedDivs[1].InnerHtml.Should().Be(htmlToEmbed);
            }

            [Fact]
            public void ShouldNotRedownloadHtmlWhenItHasAlreadyBeenDownloaded()
            {
                // Arrange
                var sourceResolver = Substitute.For<IResolveHtmlSources>();
                var htmlText = "<html><body><trns src='http://test4.com'></trns><trns src='http://test4.com'></trns></body></html>";
                var htmlParser = GetHtmlParser(sourceResolver);

                // Act
                htmlParser.Parse(htmlText);

                // Assert
                sourceResolver.Received(1).Resolve("http://test4.com");
            }

            [Fact]
            public void ShouldHandleNestedTranscludors()
            {
                // Arrange
                var outerHtml = "<p>outer html</p>";
                var innerHtml = "<p>inner html</p>";
                var expectedHtml = "<p>outer html<div><p>inner html</p></div></p>";
                var sourceResolver = Substitute.For<IResolveHtmlSources>();
                sourceResolver.Resolve("http://test6.com").Returns(outerHtml);
                sourceResolver.Resolve("http://test6.com/embedded").Returns(innerHtml);
                var htmlText = "<html><body><trns src='http://test6.com'><trns src='http://test6.com/embedded'></trns></trns></body></html>";
                var htmlParser = GetHtmlParser(sourceResolver);

                // Act
                var parsedHtml = htmlParser.Parse(htmlText);

                // Assert
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(parsedHtml);
                var embeddedDivs = htmlDoc.DocumentNode.SelectNodes("//div");
                embeddedDivs.Should().HaveCount(2);
                embeddedDivs[0].InnerHtml.Should().Be(expectedHtml);
            }

            private HtmlParser GetHtmlParser(IResolveHtmlSources resolveHtmlSources = null)
            {
                return new HtmlParser(resolveHtmlSources ?? Substitute.For<IResolveHtmlSources>(), InMemoryCacheStorage.Instance);
            }
        }
    }
}