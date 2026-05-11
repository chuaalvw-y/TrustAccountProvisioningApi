using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TrustAccountProvisioningApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                ObservabilityConfig.Register();
                ObservabilityConfig.Logger.Information(
                    "Trust Account Provisioning API application startup started.");

                DependencyConfig.Register();

                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                ObservabilityConfig.Logger.Information(
                    "Trust Account Provisioning API application startup completed successfully.");
            }
            catch (Exception ex)
            {
                ObservabilityConfig.EnsureFallbackLogger();
                ObservabilityConfig.Logger.Error(
                    ex,
                    "Trust Account Provisioning API application startup failed.");

                throw;
            }
        }

        protected void Application_End()
        {
            ObservabilityConfig.Logger.Information(
                "Trust Account Provisioning API application shutdown started.");

            DependencyConfig.Dispose();

            ObservabilityConfig.Logger.Information(
                "Trust Account Provisioning API application shutdown completed successfully.");

            ObservabilityConfig.Dispose();
        }
    }
}
