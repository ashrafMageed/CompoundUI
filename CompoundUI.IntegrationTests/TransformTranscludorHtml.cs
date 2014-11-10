using CompoundUI.Core;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.Owin.Testing;
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
}