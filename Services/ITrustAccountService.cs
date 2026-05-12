using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public interface ITrustAccountService
    {
        IEnumerable<TrustAccountResponse> Search(TrustAccountSearchRequest request);

        TrustAccountResponse Get(Guid trustAccountId);

        TrustAccountResponse Create(TrustAccountCreateRequest request);

        TrustAccountResponse Update(TrustAccountUpdateRequest request);

        bool Delete(Guid trustAccountId);

        Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest request);
    }
}
