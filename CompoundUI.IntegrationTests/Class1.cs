using System;
using System.Collections.Generic;
using CompoundUI.Core;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.Owin.Testing;
using Owin;
using Xunit;

namespace CompoundUI.IntegrationTests
{
    public class TransformTranscludorHtml : IUseFixture<OwinServer>
    {
        private TestServer _server;

        [Fact]
        public void ShouldReplaceTranscludorNodeWithDivContainingHtmlReturnedByTheSource()
        {
            // Arrange
            var htmlToEmbed = "<p>Embedded html from source</p>";
            var htmlText = "<html><body><trns src='http://testapi/TestView.html'></trns></body></html>";
            TestStartup.content.Add("http://testapi/TestView.html", htmlToEmbed);
            var htmlParser = new HtmlParser(new TestUrlHtmlSourceResolver(_server), InMemoryCacheStorage.Instance);

            // Act
            var parsedHtml = htmlParser.Parse(htmlText);

            // Assert
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(parsedHtml);
            var embeddedDivs = htmlDoc.DocumentNode.SelectNodes("//div");
            embeddedDivs.Should().HaveCount(1);
            embeddedDivs[0].InnerHtml.Should().Be(htmlToEmbed);
        }

        public void SetFixture(OwinServer data)
        {
            _server = data.Server;
        }
    }

    public class TestUrlHtmlSourceResolver : IResolveHtmlSources
    {
        private readonly TestServer _testServer;

        public TestUrlHtmlSourceResolver(TestServer testServer)
        {
            _testServer = testServer;
        }

        public string Resolve(string source)
        {
            var result = _testServer.CreateRequest(source).GetAsync().Result;
            return result.Content.ReadAsStringAsync().Result;
        }
    }

    public class TestStartup
    { 
        public static Dictionary<string,string> content = new Dictionary<string, string>();

        public void Configuration(IAppBuilder app)
        {
            app.Run(async context =>
            {
                var requestUri = context.Request.Uri.AbsoluteUri;
                await context.Response.WriteAsync(content[requestUri]);
            });
        }
    }


    public class OwinServer : IDisposable
    {
        public TestServer Server { get; set; }

        public OwinServer()
        {
            Server = TestServer.Create<TestStartup>();
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
