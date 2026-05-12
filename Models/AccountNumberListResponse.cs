using System;

namespace TrustAccountProvisioningApi.Models
{
    public class AccountNumberListResponse
    {
        public Guid AccountNumberListId { get; set; }

        public string AccountNumber { get; set; }

        public string EmployeeId { get; set; }

        public bool Manual { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}
