using System.Web.Http;
using CompoundUI.TestEndpoint.Controllers;

namespace CompoundUI.TestEndpoint
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Add(new TextPlainFormatter());
        }
    }
}
