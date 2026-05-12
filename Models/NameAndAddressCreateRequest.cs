using System;

namespace TrustAccountProvisioningApi.Models
{
    public class NameAndAddressCreateRequest
    {
        public string CustomerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public string Salutation { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }
}
