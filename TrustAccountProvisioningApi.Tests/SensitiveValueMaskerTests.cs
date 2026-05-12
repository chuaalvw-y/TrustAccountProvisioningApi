using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Tests
{
    [TestClass]
    public class SensitiveValueMaskerTests
    {
        [DataTestMethod]
        [DataRow("123456789", "*****6789")]
        [DataRow("9876", "****")]
        public void MaskLastFour_MasksSensitiveValue(string value, string expected)
        {
            Assert.AreEqual(expected, SensitiveValueMasker.MaskLastFour(value));
        }
    }
}
