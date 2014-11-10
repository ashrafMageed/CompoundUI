using System;
using HtmlAgilityPack;

namespace CompoundUI.Core
{
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
}