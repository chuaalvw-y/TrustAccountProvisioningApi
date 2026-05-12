using System;

namespace TrustAccountProvisioningApi.Models
{
    public class NameAndAddressUpdateRequest : NameAndAddressCreateRequest
    {
        public Guid NameAndAddressId { get; set; }
    }
}
