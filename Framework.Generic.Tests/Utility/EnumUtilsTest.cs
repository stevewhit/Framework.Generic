using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Framework.Generic.Utility;
using Framework.Generic.Tests.Builders;
using System.Linq;

namespace Framework.Generic.Tests.Utility
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EnumUtilsTest
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        #region Testing IEnumerable<T> GetValues<T>(this T enumType)...

        [TestMethod]
        public void GetValues_WithEmptyEnum_ReturnsEmpty()
        {
            // Act
            var enumVals = EnumUtils.GetValues<EmptyEnum>();

            // Assert
            Assert.IsTrue(enumVals.Count() == 0);
        }

        [TestMethod]
        public void GetEnumValues_WithPopulatedEnum_ReturnsValues()
        {
            // Act
            var enumVals = EnumUtils.GetValues<TestEnum>();
            var count = Enum.GetValues(typeof(TestEnum)).Cast<TestEnum>().Count();

            // Assert
            Assert.IsTrue(enumVals.Count() > 0);
            Assert.IsTrue(enumVals.Count() == count);
        }

        #endregion
        #region Testing string GetDisplayValue<T>(T value)...

        [TestMethod]
        public void GetDisplayValue_WithoutDisplayName_ReturnsEnumToString()
        {
            // Arrange
            var value = TestEnum.ValueWithoutDescriptionOrDisplay;

            // Act
            var result = EnumUtils.GetDisplayValue(value);

            // Assert
            Assert.IsTrue(result == value.ToString());
        }

        [TestMethod]
        public void GetDisplayValue_WithDisplayName_ReturnsDisplayName()
        {
            // Arrange
            var value = TestEnum.ValueWithDisplay;

            // Act
            var result = EnumUtils.GetDisplayValue(value);

            // Assert
            Assert.IsTrue(result == "Value With Display");
        }

        [TestMethod]
        public void GetDisplayValue_WithDisplayNameAndUsingExtension_ReturnsDisplayName()
        {
            // Act
            var result = TestEnum.ValueWithDisplay.GetDisplayValue();

            // Assert
            Assert.IsTrue(result == "Value With Display");
        }

        #endregion
        #region Testing IEnumerable<string> GetDisplayValues<T>()...

        [TestMethod]
        public void GetDisplayValues_WithEmptyEnum_ReturnsEmptyEnumerable()
        {
            // Act
            var result = EnumUtils.GetDisplayValues<EmptyEnum>();

            // Assert
            Assert.IsTrue(result.Count() == 0);
        }

        [TestMethod]
        public void GetDisplayValues_WithPopulatedEnum_ReturnsPopulatedEnumerable()
        {
            // Act
            var result = EnumUtils.GetDisplayValues<TestEnum>();

            // Assert
            Assert.IsTrue(result.Count() == 3);
        }

        #endregion
        #region Testing string GetDescription<T>(T value)...

        [TestMethod]
        public void GetDescription_WithEnumWithoutDescription_ReturnsEmpty()
        {
            // Arrange
            var value = TestEnum.ValueWithoutDescriptionOrDisplay;

            // Act
            var result = EnumUtils.GetDescription(value);

            // Assert
            Assert.IsTrue(result == string.Empty);
        }

        [TestMethod]
        public void GetDescription_WithEnumWithDescription_ReturnsDescription()
        {
            // Arrange
            var value = TestEnum.ValueWithDescription;

            // Act
            var result = EnumUtils.GetDescription(value);

            // Assert
            Assert.IsTrue(result == "Fake description for testing purposes.");
        }

        [TestMethod]
        public void GetDescription_WithEnumWithDescriptionUsingExtension_ReturnsDescription()
        {
            // Act
            var result = TestEnum.ValueWithDescription.GetDescription();

            // Assert
            Assert.IsTrue(result == "Fake description for testing purposes.");
        }

        #endregion
        #region Testing T Parse<T>(string value)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_WithNullValue_ThrowsException()
        {
            // Act
            var result = EnumUtils.Parse<TestEnum>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_WithEmptyValue_ThrowsException()
        {
            // Act
            var result = EnumUtils.Parse<TestEnum>("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_WithEmptyEnum_ThrowsException()
        {
            // Act
            var result = EnumUtils.Parse<EmptyEnum>("ShouldNotFind");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_WithInvalidValue_ThrowsException()
        {
            // Act
            var result = EnumUtils.Parse<TestEnum>("ShouldNotFind");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_WithInvalidValue2_ThrowsException()
        {
            // Act
            var result = EnumUtils.Parse<TestEnum>("Value With Description");
        }

        [TestMethod]
        public void Parse_WithValidValue_ReturnsEnumValue()
        {
            // Act
            var result = EnumUtils.Parse<TestEnum>("vAlUeWiThDeScRiPtIoN");

            // Assert
            Assert.AreEqual(TestEnum.ValueWithDescription, result, "Parse method should ignore the case with parsing.");
        }

        #endregion
    }
}
