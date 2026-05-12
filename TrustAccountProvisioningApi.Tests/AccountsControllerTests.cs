using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrustAccountProvisioningApi.Controllers;
using TrustAccountProvisioningApi.Models;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Tests
{
    [TestClass]
    public class AccountsControllerTests
    {
        [TestMethod]
        public async Task CreateAccount_ReturnsOkWhenStoredProcedureSucceeds()
        {
            var expected = new CreateAccountResponse
            {
                Success = true,
                AccountNumber = "1234567890",
                Message = "Created"
            };

            var controller = new AccountsController(
                new TrustAccountServiceStub(expected));

            var result = await controller.CreateAccount(
                new CreateAccountRequest { CreatedBy = "tester" });

            var okResult = result as OkNegotiatedContentResult<CreateAccountResponse>;

            Assert.IsNotNull(okResult);
            Assert.AreSame(expected, okResult.Content);
        }

        [TestMethod]
        public async Task CreateAccount_ReturnsBadRequestWhenStoredProcedureFails()
        {
            var expected = new CreateAccountResponse
            {
                Success = false,
                AccountNumber = null,
                Message = "No account numbers are available."
            };

            var controller = new AccountsController(
                new TrustAccountServiceStub(expected));

            var result = await controller.CreateAccount(
                new CreateAccountRequest { CreatedBy = "tester" });

            var badRequestResult =
                result as NegotiatedContentResult<CreateAccountResponse>;

            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.AreSame(expected, badRequestResult.Content);
        }

        [TestMethod]
        public async Task CreateAccount_ReturnsBadRequestResponseWhenValidationFails()
        {
            var controller = new AccountsController(
                new TrustAccountServiceStub(
                    new ArgumentException("CreatedBy is required.")));

            var result = await controller.CreateAccount(new CreateAccountRequest());

            var badRequestResult =
                result as NegotiatedContentResult<CreateAccountResponse>;

            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.IsFalse(badRequestResult.Content.Success);
            Assert.AreEqual("CreatedBy is required.", badRequestResult.Content.Message);
        }

        private sealed class TrustAccountServiceStub : ITrustAccountService
        {
            private readonly CreateAccountResponse _response;
            private readonly Exception _exception;

            public TrustAccountServiceStub(CreateAccountResponse response)
            {
                _response = response;
            }

            public TrustAccountServiceStub(Exception exception)
            {
                _exception = exception;
            }

            public IEnumerable<TrustAccountResponse> Search(TrustAccountSearchRequest request) =>
                throw new NotImplementedException();

            public TrustAccountResponse Get(Guid trustAccountId) =>
                throw new NotImplementedException();

            public TrustAccountResponse Create(TrustAccountCreateRequest request) =>
                throw new NotImplementedException();

            public TrustAccountResponse Update(TrustAccountUpdateRequest request) =>
                throw new NotImplementedException();

            public bool Delete(Guid trustAccountId) =>
                throw new NotImplementedException();

            public Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest request)
            {
                if (_exception != null)
                {
                    throw _exception;
                }

                return Task.FromResult(_response);
            }
        }
    }
}
