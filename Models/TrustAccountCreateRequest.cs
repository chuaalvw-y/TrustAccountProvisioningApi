using System;

namespace TrustAccountProvisioningApi.Models
{
    public class TrustAccountCreateRequest
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public string AccountStatus { get; set; }
        public DateTime? AccountOpenDate { get; set; }
        public DateTime? AccountCloseDate { get; set; }
        public string AccountDomicile { get; set; }
        public string AccountBasisName { get; set; }
        public string AccountProfitCenter { get; set; }
        public string OrganizationId { get; set; }
        public string SubdivisionId { get; set; }
        public string RelationshipId { get; set; }
        public string ProductLineId { get; set; }
        public string CapacityCode { get; set; }
        public string SimpleComplexTrust { get; set; }
        public string AccountOfficerId { get; set; }
        public string AccountOfficerName { get; set; }
        public string BackupAccountOfficerId { get; set; }
        public string SeniorAdministrator { get; set; }
        public string InvestmentOfficerId { get; set; }
        public string InvestmentOfficerName { get; set; }
        public string InvestmentObjective { get; set; }
        public string PortfolioModel { get; set; }
        public string PortfolioType { get; set; }
        public bool AccrualRequiredFlag { get; set; }
        public string AccretionMethod { get; set; }
        public string AccretionMethodModelId { get; set; }
        public string AmortizationMethod { get; set; }
        public string TaxId { get; set; }
        public string TaxIdType { get; set; }
        public string TaxJurisdiction { get; set; }
        public string TaxFederalProfile { get; set; }
        public string FiscalYearEnd { get; set; }
        public string TaxYearEndMonthNumber { get; set; }
        public bool NonResidentAlienWithholdingFlag { get; set; }
        public bool ExemptForeignWithholdingFlag { get; set; }
        public bool Us1099ReportFlag { get; set; }
        public bool Prepare1099ReturnFlag { get; set; }
        public string CreatedBy { get; set; }
    }
}
