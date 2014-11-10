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

            private HtmlParser GetHtmlParser(IResolveHtmlSources resolveHtmlSources = null)
            {
                return new HtmlParser(resolveHtmlSources ?? Substitute.For<IResolveHtmlSources>(), InMemoryCacheStorage.Instance);
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
        private readonly ICacheStorage _cacheStorage;

        public HtmlParser(IResolveHtmlSources htmlSourcesResolver, ICacheStorage cacheStorage)
        {
            _htmlSourcesResolver = htmlSourcesResolver;
            _cacheStorage = cacheStorage;
        }

        public string Parse(string htmlText)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlText);
            var transcludorNodes = htmlDocument.DocumentNode.SelectNodes("//trns");
            foreach (var transcludorNode in transcludorNodes)
            {
                var htmlSource = transcludorNode.Attributes["src"].Value;
                var html = _cacheStorage.Get(htmlSource, () => _htmlSourcesResolver.Resolve(htmlSource));
                var newNode = HtmlNode.CreateNode(String.Format("<div>{0}</div>", html));
                transcludorNode.ParentNode.ReplaceChild(newNode, transcludorNode);
            }

            return htmlDocument.DocumentNode.OuterHtml;
        }

    }

    public sealed class InMemoryCacheStorage : ICacheStorage
    {
        private static readonly Lazy<InMemoryCacheStorage>  _lazy = new Lazy<InMemoryCacheStorage>(() => new InMemoryCacheStorage());

        private static readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        public static InMemoryCacheStorage Instance { get { return _lazy.Value; }}

        private InMemoryCacheStorage() {}

        public T Get<T>(string key, Func<T> getWhenCacheMiss)
        {
            if (_storage.ContainsKey(key))
                return (T)_storage[key];

            var item = getWhenCacheMiss();
            _storage.Add(key, item);
            return item;
        }
    }

    public interface ICacheStorage
    {
        T Get<T>(string key, Func<T> getWhenCacheMiss);
    }
}