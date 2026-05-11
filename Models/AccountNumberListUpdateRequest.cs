using System;

namespace TrustAccountProvisioningApi.Models
{
    public class AccountNumberListUpdateRequest
    {
        public Guid AccountNumberListId { get; set; }

        public string AccountNumber { get; set; }

        public string EmployeeId { get; set; }

        public bool Manual { get; set; }

        public string Notes { get; set; }

        public string ModifiedBy { get; set; }
    }
}
