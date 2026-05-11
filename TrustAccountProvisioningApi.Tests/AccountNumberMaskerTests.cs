using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi.Tests
{
    [TestClass]
    public class AccountNumberMaskerTests
    {
        [DataTestMethod]
        [DataRow("1234567890", "******7890")]
        [DataRow("1234", "****")]
        [DataRow("12", "**")]
        [DataRow("1", "*")]
        public void Mask_MasksAllButLastFourCharacters(string accountNumber, string expected)
        {
            Assert.AreEqual(expected, AccountNumberMasker.Mask(accountNumber));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Mask_ReturnsNullForBlankValues(string accountNumber)
        {
            Assert.IsNull(AccountNumberMasker.Mask(accountNumber));
        }
    }
}
