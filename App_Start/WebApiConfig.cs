using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ChuA.ObservabilityLegacy.Extensions;

namespace TrustAccountProvisioningApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.UseChuAExceptionHandling();
            config.UseChuARequestLogging();
            SwaggerConfig.Register(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
