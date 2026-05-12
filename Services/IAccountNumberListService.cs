using System;
using System.Collections.Generic;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public interface IAccountNumberListService
    {
        IEnumerable<AccountNumberListResponse> Search(AccountNumberListSearchRequest request);

        AccountNumberListResponse Get(Guid accountNumberListId);

        AccountNumberListResponse Create(AccountNumberListCreateRequest request);

        AccountNumberListResponse Update(AccountNumberListUpdateRequest request);

        bool Delete(Guid accountNumberListId);
    }
}
