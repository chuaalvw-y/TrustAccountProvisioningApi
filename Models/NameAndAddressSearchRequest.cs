namespace TrustAccountProvisioningApi.Models
{
    public class NameAndAddressSearchRequest
    {
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public bool? IsActive { get; set; }
    }
}
