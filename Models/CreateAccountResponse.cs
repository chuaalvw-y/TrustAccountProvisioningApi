namespace TrustAccountProvisioningApi.Models
{
    public class CreateAccountResponse
    {
        public bool Success { get; set; }

        public string AccountNumber { get; set; }

        public string Message { get; set; }
    }
}
