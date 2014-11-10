using System;
using System.Collections.Generic;
using CompoundUI.Core;
using Microsoft.Owin.Testing;
using Owin;

namespace CompoundUI.IntegrationTests
{
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
