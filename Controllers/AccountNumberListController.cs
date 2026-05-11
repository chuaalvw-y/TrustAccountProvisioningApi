using System;
using System.Web.Http;
using TrustAccountProvisioningApi.Models;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Controllers
{
    [RoutePrefix("api/account-number-list")]
    public class AccountNumberListController : ApiController
    {
        private readonly IAccountNumberListService _accountNumberListService;

        public AccountNumberListController(IAccountNumberListService accountNumberListService)
        {
            _accountNumberListService = accountNumberListService
                ?? throw new ArgumentNullException(nameof(accountNumberListService));
        }

        [HttpPost]
        [Route("search")]
        public IHttpActionResult Search(AccountNumberListSearchRequest request)
        {
            return Ok(_accountNumberListService.Search(request));
        }

        [HttpPost]
        [Route("get")]
        public IHttpActionResult Get(AccountNumberListGetRequest request)
        {
            if (request == null || request.AccountNumberListId == Guid.Empty)
            {
                return BadRequest("AccountNumberListId is required.");
            }

            var accountNumber = _accountNumberListService.Get(request.AccountNumberListId);
            return accountNumber == null
                ? (IHttpActionResult)NotFound()
                : Ok(accountNumber);
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(AccountNumberListCreateRequest request)
        {
            try
            {
                return Ok(_accountNumberListService.Create(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(AccountNumberListUpdateRequest request)
        {
            try
            {
                var accountNumber = _accountNumberListService.Update(request);
                return accountNumber == null
                    ? (IHttpActionResult)NotFound()
                    : Ok(accountNumber);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("delete")]
        public IHttpActionResult Delete(AccountNumberListDeleteRequest request)
        {
            if (request == null || request.AccountNumberListId == Guid.Empty)
            {
                return BadRequest("AccountNumberListId is required.");
            }

            return _accountNumberListService.Delete(request.AccountNumberListId)
                ? (IHttpActionResult)Ok()
                : NotFound();
        }
    }
}
