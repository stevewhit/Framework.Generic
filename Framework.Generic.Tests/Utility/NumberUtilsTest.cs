using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Generic.Utility;

namespace Framework.Generic.Tests.Utility
{
    [TestClass]
    public class NumberUtilsTest
    {
        #region Testing int GenerateRandomNumber(int min, int max)...

        [TestMethod]
        public void GenerateRandomNumber_WithValidMinMax_ReturnsAllValuesInRangeInclusive()
        {
            // Arrange
            var randNumDict = new Dictionary<int, int>();
            var min = 0;
            var max = 100;

            // Act
            for (int i = 0; i < 10000; i++)
            {
                var rand = NumberUtils.GenerateRandomNumber(0, 100);

                if (!randNumDict.ContainsKey(rand))
                    randNumDict.Add(rand, 0);

                randNumDict[rand]++;
            }

            randNumDict = randNumDict.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Assert
            for (int i = min; i <= max; i++)
                Assert.IsTrue(randNumDict.ContainsKey(min));
        }

        #endregion
    }
}
