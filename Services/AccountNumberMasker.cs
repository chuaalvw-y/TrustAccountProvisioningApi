namespace TrustAccountProvisioningApi.Services
{
    internal static class AccountNumberMasker
    {
        public static string Mask(string accountNumber)
        {
            return SensitiveValueMasker.MaskLastFour(accountNumber);
        }
    }
}
