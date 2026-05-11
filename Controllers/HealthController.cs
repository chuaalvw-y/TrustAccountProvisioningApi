using System;
using System.Web.Http;

namespace TrustAccountProvisioningApi.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult Check()
        {
            return Ok(new
            {
                status = "Healthy",
                checkedAtUtc = DateTime.UtcNow
            });
        }
    }
}
