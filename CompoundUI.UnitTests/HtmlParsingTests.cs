using System;
using System.Collections.Generic;
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
                var parsedHtml = htmlParser.Parse(htmlText);

                // Assert
                parsedHtml.Should().Contain("div");
            }

            [Fact]
            public void ShouldReplaceAllTransclusionElementsWithDivs()
            {
                // Arrange
                var htmlWith3ReplacableSections = "<html><body><trns src='http://test.com'></trns><p>Test</p><trns src='http://test.com'></trns><trns src='http://test.com'></trns></body></html>";
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
                sourceResolver.Resolve("http://test.com").Returns(htmlToEmbed);
                var htmlText = "<html><body><trns src='http://test.com'></trns><trns src='http://test.com'></trns></body></html>";
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
                var htmlText = "<html><body><trns src='http://test.com'></trns><trns src='http://test.com'></trns></body></html>";
                var htmlParser = GetHtmlParser(sourceResolver);

                // Act
                htmlParser.Parse(htmlText);

                // Assert
                sourceResolver.Received(1).Resolve("http://test.com");
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
        private Dictionary<string, string> content = new Dictionary<string, string>(); 
        private readonly IResolveHtmlSources _htmlSourcesResolver;
        private HtmlDocument _htmlDocument;

        public HtmlParser(IResolveHtmlSources htmlSourcesResolver)
        {
            _htmlSourcesResolver = htmlSourcesResolver;
        }

        public string Parse(string htmlText)
        {
            _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(htmlText);
            var transcludorNodes = _htmlDocument.DocumentNode.SelectNodes("//trns");
            foreach (var transcludorNode in transcludorNodes)
            {
                var htmlSource = transcludorNode.Attributes["src"].Value;
                var html = content.ContainsKey(htmlSource) ? content[htmlSource] : _htmlSourcesResolver.Resolve(htmlSource);
                if(!content.ContainsKey(htmlSource)) content.Add(htmlSource, html);
                var newNode = HtmlNode.CreateNode(String.Format("<div>{0}</div>", html));
                transcludorNode.ParentNode.ReplaceChild(newNode, transcludorNode);
            }

            return _htmlDocument.DocumentNode.OuterHtml;
        }

    }

    public interface ICacheStorage
    {
        T Get<T>(string key);
        void Add<T>(string key, T item);
    }
}