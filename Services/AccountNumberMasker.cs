namespace TrustAccountProvisioningApi.Services
{
    internal static class AccountNumberMasker
    {
        public static string Mask(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return null;
            }

            if (accountNumber.Length <= 4)
            {
                return new string('*', accountNumber.Length);
            }

            return new string('*', accountNumber.Length - 4) +
                accountNumber.Substring(accountNumber.Length - 4);
        }
    }
}
