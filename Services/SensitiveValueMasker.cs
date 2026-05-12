namespace TrustAccountProvisioningApi.Services
{
    internal static class SensitiveValueMasker
    {
        public static string MaskLastFour(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.Length <= 4)
            {
                return new string('*', value.Length);
            }

            return new string('*', value.Length - 4) +
                value.Substring(value.Length - 4);
        }
    }
}
