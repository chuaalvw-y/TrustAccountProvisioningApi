using System;
using System.Web.Http;
using TrustAccountProvisioningApi.Models;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Controllers
{
    [RoutePrefix("api/trust-account")]
    public class TrustAccountController : ApiController
    {
        private readonly ITrustAccountService _trustAccountService;

        public TrustAccountController(ITrustAccountService trustAccountService)
        {
            _trustAccountService = trustAccountService
                ?? throw new ArgumentNullException(nameof(trustAccountService));
        }

        [HttpPost]
        [Route("search")]
        public IHttpActionResult Search(TrustAccountSearchRequest request)
        {
            return Ok(_trustAccountService.Search(request));
        }

        [HttpPost]
        [Route("get")]
        public IHttpActionResult Get(IdRequest request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Id is required.");
            }

            var trustAccount = _trustAccountService.Get(request.Id);
            return trustAccount == null
                ? (IHttpActionResult)NotFound()
                : Ok(trustAccount);
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(TrustAccountCreateRequest request)
        {
            try
            {
                return Ok(_trustAccountService.Create(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(TrustAccountUpdateRequest request)
        {
            try
            {
                var trustAccount = _trustAccountService.Update(request);
                return trustAccount == null
                    ? (IHttpActionResult)NotFound()
                    : Ok(trustAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("delete")]
        public IHttpActionResult Delete(IdRequest request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Id is required.");
            }

            return _trustAccountService.Delete(request.Id)
                ? (IHttpActionResult)Ok()
                : NotFound();
        }
    }
}
