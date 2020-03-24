using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Framework.Generic.Utility;

namespace Framework.Generic.Tests.Utility
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SystemTimeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            
        }
        #region Testing Property SystemTime.Now()

        [TestMethod]
        public void Now_ReturnsDateTimeNow()
        {
            // Assert
            Assert.IsTrue(SystemTime.Now().Date == DateTime.Now.Date);
        }

        #endregion
        #region Testing void SetDateTime(DateTime dateTimeNow)

        [TestMethod]
        public void SetDateTime_WithValidDateTime_SetsDateTime()
        {
            // Arrange
            var fakeTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 0);

            // Act
            SystemTime.SetDateTime(fakeTime);

            // Assert
            Assert.IsTrue(SystemTime.Now() == fakeTime);
        }

        #endregion
        #region Testing void ResetDateTime()

        [TestMethod]
        public void ResetDateTime_ResetsDateTime()
        {
            // Arrange
            var fakeTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 0);
            SystemTime.SetDateTime(fakeTime);

            // Act
            SystemTime.ResetDateTime();

            // Assert
            Assert.IsTrue(SystemTime.Now() != fakeTime);
        }

        #endregion  
    }
}
