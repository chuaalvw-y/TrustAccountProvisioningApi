using System;
using System.Web.Http;
using TrustAccountProvisioningApi.Models;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Controllers
{
    [RoutePrefix("api/name-and-address")]
    public class NameAndAddressController : ApiController
    {
        private readonly INameAndAddressService _nameAndAddressService;

        public NameAndAddressController(INameAndAddressService nameAndAddressService)
        {
            _nameAndAddressService = nameAndAddressService
                ?? throw new ArgumentNullException(nameof(nameAndAddressService));
        }

        [HttpPost]
        [Route("search")]
        public IHttpActionResult Search(NameAndAddressSearchRequest request)
        {
            return Ok(_nameAndAddressService.Search(request));
        }

        [HttpPost]
        [Route("get")]
        public IHttpActionResult Get(IdRequest request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Id is required.");
            }

            var nameAndAddress = _nameAndAddressService.Get(request.Id);
            return nameAndAddress == null
                ? (IHttpActionResult)NotFound()
                : Ok(nameAndAddress);
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(NameAndAddressCreateRequest request)
        {
            try
            {
                return Ok(_nameAndAddressService.Create(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(NameAndAddressUpdateRequest request)
        {
            try
            {
                var nameAndAddress = _nameAndAddressService.Update(request);
                return nameAndAddress == null
                    ? (IHttpActionResult)NotFound()
                    : Ok(nameAndAddress);
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

            return _nameAndAddressService.Delete(request.Id)
                ? (IHttpActionResult)Ok()
                : NotFound();
        }
    }
}
