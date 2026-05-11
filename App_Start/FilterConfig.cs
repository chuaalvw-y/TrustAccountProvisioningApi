using System.Web;
using System.Web.Mvc;
using ChuA.ObservabilityLegacy.Extensions;

namespace TrustAccountProvisioningApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.UseChuAExceptionHandling();
            filters.UseChuARequestLogging();
        }
    }
}
