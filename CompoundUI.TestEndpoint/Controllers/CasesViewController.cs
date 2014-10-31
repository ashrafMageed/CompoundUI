using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace CompoundUI.TestEndpoint.Controllers
{
    public class CasesViewController : ApiController
    {
        [Route("CasesView/{relativePath}")]
        public HttpResponseMessage Get(string relativePath)
        {
            var pageStringBuilder = new StringBuilder();
            pageStringBuilder.Append(File.ReadAllText(@"C:\Users\mageas\Documents\GitHub\CompoundUI\CompoundUI.TestEndpoint\Content\Controllers\CaseDetailsController.js"));
            pageStringBuilder.Append(File.ReadAllText(@"C:\Users\mageas\Documents\GitHub\CompoundUI\CompoundUI.TestEndpoint\Content\Services\CasesService.js"));
            pageStringBuilder.Append(File.ReadAllText(@"C:\Users\mageas\Documents\GitHub\CompoundUI\CompoundUI.TestEndpoint\Content\Views\CaseDetails.html"));
            var responseBody = pageStringBuilder.ToString();
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, responseBody, new TextPlainFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }
    }

    public class TextPlainFormatter : BufferedMediaTypeFormatter
    {
        public TextPlainFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var writer = new StreamWriter(writeStream);
            writer.Write(value);
            writer.Flush();
        }
    }

    
//    public class HtmlActionResult : IHttpActionResult
//    {
//        private const string ViewDirectory = @"C:\git\FormsPOC\WebApplication1\Views";
//        private readonly string _view;
//        private readonly dynamic _model;
//
//        public HtmlActionResult(string viewName, dynamic model)
//        {
//            _view = LoadView(viewName);
//            _model = model;
//        }
//
//        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//        {
//            var response = new HttpResponseMessage(HttpStatusCode.OK);
//            var parsedView = PageGenerator.GenerateFormPage(_model, _view);
//            response.Content = new StringContent(parsedView);
//            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
//            return Task.FromResult(response);
//        }
//
//        private static string LoadView(string name)
//        {
//            var view = File.ReadAllText(Path.Combine(ViewDirectory, name + ".cshtml"));
//            return view;
//        }
//    }
}
