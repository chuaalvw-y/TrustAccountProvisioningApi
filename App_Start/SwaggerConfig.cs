using System.Web.Http;
using Swashbuckle.Application;

namespace TrustAccountProvisioningApi
{
    public static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Trust Account Provisioning API");
                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi();
        }
    }
}
