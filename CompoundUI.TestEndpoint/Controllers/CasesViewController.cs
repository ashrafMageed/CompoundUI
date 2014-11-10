using System;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Net.Http;
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
            pageStringBuilder.Append(EncloseInScriptTags(File.ReadAllText(@"C:\Users\mageas\Documents\GitHub\CompoundUI\CompoundUI.TestEndpoint\Content\Controllers\CaseDetailsController.js")));
            pageStringBuilder.Append(EncloseInScriptTags(File.ReadAllText(@"C:\Users\mageas\Documents\GitHub\CompoundUI\CompoundUI.TestEndpoint\Content\Services\CasesService.js")));
            pageStringBuilder.Append(File.ReadAllText(@"C:\Users\mageas\Documents\GitHub\CompoundUI\CompoundUI.TestEndpoint\Content\Views\CaseDetails.html"));
            var responseBody = pageStringBuilder.ToString();
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, responseBody, new TextPlainFormatter());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
            return response;
        }

        private string EncloseInScriptTags(string js)
        {
            return String.Format("<script type='text/javascript'>{0}</script>", js);
        }

    }



}
