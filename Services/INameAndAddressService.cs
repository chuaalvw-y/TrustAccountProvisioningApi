using System;
using System.Collections.Generic;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public interface INameAndAddressService
    {
        IEnumerable<NameAndAddressResponse> Search(NameAndAddressSearchRequest request);

        NameAndAddressResponse Get(Guid nameAndAddressId);

        NameAndAddressResponse Create(NameAndAddressCreateRequest request);

        NameAndAddressResponse Update(NameAndAddressUpdateRequest request);

        bool Delete(Guid nameAndAddressId);
    }
}
