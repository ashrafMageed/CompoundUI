using System;
using System.Configuration;
using Capabilities.Host;
using CompoundUI.Composition;
using Microsoft.Owin.Hosting;

namespace CompoundUI.CompositionHost
{
    public class CompositionHost : IHostService, ISpecifyServiceDetails
    {
        private IDisposable _server;

        public void StartService()
        {
            _server = WebApp.Start<Startup>(new StartOptions(ConfigurationManager.AppSettings["HostUrl"]));
        }

        public void StopService()
        {
            _server.Dispose();
        }

        public string ServiceDisplayName
        {
            get { return "CU.Composition"; }
        }

        public string ServiceName
        {
            get { return "CU.Composition"; }
        }

        public string ServiceDescription
        {
            get { return "UI Composition Host"; }
        }
    }
}
