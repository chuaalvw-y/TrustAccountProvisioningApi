using System;

namespace TrustAccountProvisioningApi.Models
{
    public class TrustAccountUpdateRequest : TrustAccountCreateRequest
    {
        public Guid TrustAccountId { get; set; }

        public string ModifiedBy { get; set; }
    }
}
