using System;
using System.Collections.Generic;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public interface IAccountNumberListService
    {
        IEnumerable<AccountNumberListDto> Search(AccountNumberListSearchRequest request);

        AccountNumberListDto Get(Guid accountNumberListId);

        AccountNumberListDto Create(AccountNumberListCreateRequest request);

        AccountNumberListDto Update(AccountNumberListUpdateRequest request);

        bool Delete(Guid accountNumberListId);
    }
}
