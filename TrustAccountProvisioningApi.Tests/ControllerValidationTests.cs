using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrustAccountProvisioningApi.Controllers;
using TrustAccountProvisioningApi.Models;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Tests
{
    [TestClass]
    public class ControllerValidationTests
    {
        [TestMethod]
        public void AccountNumberList_Get_ReturnsBadRequestForMissingId()
        {
            var controller = new AccountNumberListController(new AccountNumberListServiceStub());

            Assert.IsInstanceOfType(
                controller.Get(new IdRequest()),
                typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void AccountNumberList_Delete_ReturnsBadRequestForMissingId()
        {
            var controller = new AccountNumberListController(new AccountNumberListServiceStub());

            Assert.IsInstanceOfType(
                controller.Delete(null),
                typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void TrustAccount_Get_ReturnsBadRequestForMissingId()
        {
            var controller = new TrustAccountController(new TrustAccountServiceStub());

            Assert.IsInstanceOfType(
                controller.Get(new IdRequest()),
                typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void TrustAccount_Delete_ReturnsBadRequestForMissingId()
        {
            var controller = new TrustAccountController(new TrustAccountServiceStub());

            Assert.IsInstanceOfType(
                controller.Delete(null),
                typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void NameAndAddress_Get_ReturnsBadRequestForMissingId()
        {
            var controller = new NameAndAddressController(new NameAndAddressServiceStub());

            Assert.IsInstanceOfType(
                controller.Get(new IdRequest()),
                typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void NameAndAddress_Delete_ReturnsBadRequestForMissingId()
        {
            var controller = new NameAndAddressController(new NameAndAddressServiceStub());

            Assert.IsInstanceOfType(
                controller.Delete(null),
                typeof(BadRequestErrorMessageResult));
        }

        private sealed class AccountNumberListServiceStub : IAccountNumberListService
        {
            public IEnumerable<AccountNumberListResponse> Search(AccountNumberListSearchRequest request) =>
                throw new NotImplementedException();

            public AccountNumberListResponse Get(Guid accountNumberListId) =>
                throw new NotImplementedException();

            public AccountNumberListResponse Create(AccountNumberListCreateRequest request) =>
                throw new NotImplementedException();

            public AccountNumberListResponse Update(AccountNumberListUpdateRequest request) =>
                throw new NotImplementedException();

            public bool Delete(Guid accountNumberListId) =>
                throw new NotImplementedException();
        }

        private sealed class TrustAccountServiceStub : ITrustAccountService
        {
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

            public Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest request) =>
                throw new NotImplementedException();
        }

        private sealed class NameAndAddressServiceStub : INameAndAddressService
        {
            public IEnumerable<NameAndAddressResponse> Search(NameAndAddressSearchRequest request) =>
                throw new NotImplementedException();

            public NameAndAddressResponse Get(Guid nameAndAddressId) =>
                throw new NotImplementedException();

            public NameAndAddressResponse Create(NameAndAddressCreateRequest request) =>
                throw new NotImplementedException();

            public NameAndAddressResponse Update(NameAndAddressUpdateRequest request) =>
                throw new NotImplementedException();

            public bool Delete(Guid nameAndAddressId) =>
                throw new NotImplementedException();
        }
    }
}
