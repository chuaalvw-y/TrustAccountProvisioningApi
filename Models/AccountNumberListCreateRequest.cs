namespace TrustAccountProvisioningApi.Models
{
    public class AccountNumberListCreateRequest
    {
        public string AccountNumber { get; set; }

        public string EmployeeId { get; set; }

        public bool Manual { get; set; }

        public string Notes { get; set; }

        public string CreatedBy { get; set; }
    }
}
