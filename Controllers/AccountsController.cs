using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TrustAccountProvisioningApi.Models;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiController
    {
        private readonly ITrustAccountService _trustAccountService;

        public AccountsController(ITrustAccountService trustAccountService)
        {
            _trustAccountService = trustAccountService
                ?? throw new ArgumentNullException(nameof(trustAccountService));
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateAccount(CreateAccountRequest request)
        {
            try
            {
                var response = await _trustAccountService.CreateAccountAsync(request);

                return response.Success
                    ? (IHttpActionResult)Ok(response)
                    : Content(HttpStatusCode.BadRequest, response);
            }
            catch (ArgumentException ex)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    new CreateAccountResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
        }
    }
}
