using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace CompoundUI.TestEndpoint.Controllers
{
    public class CasesViewController : ApiController
    {
        [Route("CasesView/{viewName}")]
        public HttpResponseMessage Get(string viewName)
        {
            var applicationPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var pagesDirectory = Path.Combine(applicationPath, @"Content\");
            var pageStringBuilder = new StringBuilder();
            pageStringBuilder.Append(EncloseInScriptTags("var CMAPP = angular.module('CaseManagement.App', []);"));
            pageStringBuilder.Append(EncloseInScriptTags(File.ReadAllText(String.Format(@"{0}\Controllers\{1}Controller.js", pagesDirectory, viewName))));
            pageStringBuilder.Append(EncloseInScriptTags(GetServicesFile(pagesDirectory, viewName)));
            pageStringBuilder.Append(File.ReadAllText(String.Format(@"{0}\Views\{1}.html", pagesDirectory, viewName)));
            var responseBody = pageStringBuilder.ToString();
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, responseBody, new TextPlainFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }

        private string EncloseInScriptTags(string js)
        {
            return String.Format("<script type='text/javascript'>{0}</script>", js);
        }

        private string GetServicesFile(string pagesDirectory, string viewName)
        {
            if (File.Exists(String.Format(@"{0}\Services\{1}Service.js", pagesDirectory, viewName)))
                return File.ReadAllText(String.Format(@"{0}\Services\{1}Service.js", pagesDirectory, viewName));

            if (Directory.GetFiles(pagesDirectory).Count() == 1)
                return File.ReadAllText(Directory.GetFiles(pagesDirectory).First());

            throw new Exception("Service filecould not be found");

        }

    }



}
