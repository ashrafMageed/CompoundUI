using System;
using System.Collections.Generic;
using System.Linq;
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
                var htmlComposer = GetHtmlComposer();
                var htmlParser = new HtmlParser(htmlText);

                // Act
                htmlComposer.Compose(htmlParser);

                // Assert
                htmlParser.GetParsedHtmlString().Should().Contain("div");
            }

            [Fact]
            public void ShouldReplaceAllTransclusionElementsWithDivs()
            {
                // Arrange
                var htmlWith3ReplacableSections = "<html><body><trns src='http://test.com'></trns><p>Test</p><trns src='http://test.com'></trns><trns src='http://test.com'></trns></body></html>";
                var htmlComposer = GetHtmlComposer();
                var htmlParser = new HtmlParser(htmlWith3ReplacableSections);

                // Act
                htmlComposer.Compose(new HtmlParser(htmlWith3ReplacableSections));

                // Assert
                var embeddedDivs = htmlParser.GetParsedHtmlDocument().DocumentNode.SelectNodes("//div");
                embeddedDivs.Should().HaveCount(3);
            }
//
//            [Fact]
//            public void ShouldReplaceAllTransclusionElementsContentFromSpecifiedUrlSrc()
//            {
//                // Arrange
//                var htmlToEmbed = "<p>Embedded html from source</p>";
//                var sourceResolver = Substitute.For<IResolveHtmlSources>();
//                sourceResolver.Resolve("http://test.com").Returns(htmlToEmbed);
//                var htmlText = "<html><body><trns src='http://test.com'></trns></body></html>";
//                var htmlParser = GetHtmlComposer(sourceResolver);
//
//                // Act
//                htmlParser.Parse(htmlText);
//
//                // Assert
//                htmlParser.GetParsedHtml().Should().Contain(htmlToEmbed);
//            }

            private HtmlComposer GetHtmlComposer(IResolveHtmlSources resolveHtmlSources = null)
            {
                return new HtmlComposer(resolveHtmlSources ?? Substitute.For<IResolveHtmlSources>());
            }
        }
    }

    public interface IResolveHtmlSources
    {
        string Resolve(string source);
    }

    public class HtmlParser
    {
        private HtmlDocument _htmlDocument;

        public HtmlParser(string html)
        {
            Parse(html);
        }

        private void Parse(string htmlText)
        {
            _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(htmlText);
        }

        public HtmlDocument GetParsedHtmlDocument()
        {
            return _htmlDocument;
        }

        public string GetParsedHtmlString()
        {
            return _htmlDocument.DocumentNode.OuterHtml;
        }
    }

    public static class TranscludorNodesParser
    {

        public static IEnumerable<Uri> GetHtmlSourceFromDocument(HtmlDocument htmlDocument)
        {
            var transcludorNodes = htmlDocument.DocumentNode.SelectNodes("//trns");
            return transcludorNodes.Select(transcludorNode => new Uri(transcludorNode.Attributes["src"].Value));
        }

        public static void InjectContentIntoDocument(Dictionary<string, string> contentList, HtmlDocument htmlDocument)
        {
            var transcludorNodes = htmlDocument.DocumentNode.SelectNodes("//trns");
            foreach (var content in contentList)
            {
                var content1 = content;
                var node = transcludorNodes.Select(n => n.SelectSingleNode(String.Format("//trns[@src='{0}']", content1.Key))).First();
                var newNode = HtmlNode.CreateNode(String.Format("<div>{0}</div>", content.Value));
                node.ParentNode.ReplaceChild(newNode, node);
            }
        }

    }

    public class HtmlComposer
    {
        private readonly IResolveHtmlSources _htmlSourcesResolver;

        public HtmlComposer(IResolveHtmlSources htmlSourcesResolver)
        {
            _htmlSourcesResolver = htmlSourcesResolver;
        }

        public void Compose(HtmlParser parser)
        {
            var sources = TranscludorNodesParser.GetHtmlSourceFromDocument(parser.GetParsedHtmlDocument());
            var content = sources.ToDictionary(d => d.OriginalString, source => _htmlSourcesResolver.Resolve(source.AbsoluteUri));
            TranscludorNodesParser.InjectContentIntoDocument(content, parser.GetParsedHtmlDocument());
        }
    }

//    public interface IResolveHtmlSources
//    {
//        string Resolve(string source);
//    }
//
//    public class HtmlParser
//    {
//        private readonly IResolveHtmlSources _htmlSourcesResolver;
//        private HtmlDocument _htmlDocument;
//
//        public HtmlParser(IResolveHtmlSources htmlSourcesResolver)
//        {
//            _htmlSourcesResolver = htmlSourcesResolver;
//        }
//
//        public void Parse(string htmlText)
//        {
//            _htmlDocument = new HtmlDocument();
//            _htmlDocument.LoadHtml(htmlText);
//            var transcludorNodes = _htmlDocument.DocumentNode.SelectNodes("//trns");
//            foreach (var transcludorNode in transcludorNodes)
//            {
//                var htmlSource = transcludorNode.Attributes["src"].Value;
//                var html = _htmlSourcesResolver.Resolve(htmlSource);
//                var newNode = HtmlNode.CreateNode(String.Format("<div>{0}</div>", html));
//                transcludorNode.ParentNode.ReplaceChild(newNode, transcludorNode);
//            }
//        }
//
//        public string GetParsedHtml()
//        {
//            return _htmlDocument.DocumentNode.OuterHtml;
//        }
//
//    }
}