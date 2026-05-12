namespace TrustAccountProvisioningApi.Models
{
    public class TrustAccountSearchRequest
    {
        public string AccountNumber { get; set; }
        public string AccountStatus { get; set; }
        public string OrganizationId { get; set; }
        public string RelationshipId { get; set; }
        public string AccountOfficerId { get; set; }
    }
}
