namespace TrustAccountProvisioningApi.Services
{
    internal static class SqlColumnList
    {
        public static string ToInsertedColumns(string columns)
        {
            return columns.Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace(" ", string.Empty)
                .Replace(",", ", inserted.")
                .Insert(0, "inserted.");
        }
    }
}
