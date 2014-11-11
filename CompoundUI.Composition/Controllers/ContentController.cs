using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CompoundUI.Core;

namespace CompoundUI.Composition.Controllers
{
    public class ContentController : ApiController
    {
        [Route("Page/{pageName}")]
        public HtmlActionResult Get(string pageName)
        {
            var applicationPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var pagesDirectory = Path.Combine(applicationPath, @"Content\Pages");
            var pageMarkup = File.ReadAllText(String.Format(@"{0}\{1}.html", pagesDirectory, pageName));
            var html = new HtmlParser(new UrlHtmlSourceResolver(), InMemoryCacheStorage.Instance);
            var responseBody = html.Parse(pageMarkup);
            return new HtmlActionResult(responseBody);
        }
    }

        public class HtmlActionResult : IHttpActionResult
        {
            private readonly string _htmlContent;
    
            public HtmlActionResult(string htmlContent)
            {
                _htmlContent = htmlContent;
            }
    
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(_htmlContent);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return Task.FromResult(response);
            }
        }
}
