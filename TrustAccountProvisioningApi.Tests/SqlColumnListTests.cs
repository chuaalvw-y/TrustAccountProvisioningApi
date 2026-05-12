using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Tests
{
    [TestClass]
    public class SqlColumnListTests
    {
        [TestMethod]
        public void ToInsertedColumns_PrefixesEachColumnWithInserted()
        {
            var columns = @"
TrustAccountId,
AccountNumber,
CreatedDate";

            var result = SqlColumnList.ToInsertedColumns(columns);

            Assert.AreEqual(
                "inserted.TrustAccountId, inserted.AccountNumber, inserted.CreatedDate",
                result);
        }
    }
}
