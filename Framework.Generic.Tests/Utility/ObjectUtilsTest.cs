using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Framework.Generic.Utility;
using Framework.Generic.Tests.Builders;
using System.Collections.Generic;

namespace Framework.Generic.Tests.Utility
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ObjectUtilsTest
    {
        private CloneableObject _obj;
        private CloneableObjectWithNestedClass _advObj;

        [TestInitialize]
        public void Initialize()
        {
            _obj = new CloneableObject();
            _advObj = new CloneableObjectWithNestedClass();
        }

        #region Testing T CopyObject<T>(this T source)...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CopyObject_WithNullSource_ThrowsException()
        {
            // Arrange
            _obj = null;

            // Act
            _obj.CopyObject();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyObject_ThatIsntSerializable_ThrowsException()
        {
            // Arrange
            var nonSerializableObj = new NotSerializableObject();

            // Act
            nonSerializableObj.CopyObject();
        }

        [TestMethod]
        public void CopyObject_WithPrivateVariable_CopiesVariable()
        {
            // Arrange
            _obj.SetPrivateNumber(CloneableObject.Numbers.Two);

            // Act
            var clone = _obj.CopyObject();

            // Assert
            Assert.IsTrue(clone.GetPrivateNumber() == CloneableObject.Numbers.Two);
        }

        [TestMethod]
        public void CopyObject_WithPublicProperties_CopiesProperty()
        {
            // Arrange
            _obj.PublicNumber = CloneableObject.Numbers.One;

            // Act
            var clone = _obj.CopyObject();

            // Assert
            Assert.IsTrue(clone.PublicNumber == CloneableObject.Numbers.One);
        }

        [TestMethod]
        public void CopyObject_WithChangedPrivateProperties_CopiesProperty()
        {
            // Arrange
            _obj.SetPrivateNumber(CloneableObject.Numbers.Two);
            
            // Act
            var clone = _obj.CopyObject();

            // Assert
            Assert.IsTrue(clone.GetPrivateNumber() == CloneableObject.Numbers.Two);
        }

        [TestMethod]
        public void CopyObject_WithChangedPublicProperties_CopiesProperty()
        {
            // Arrange
            _obj.PublicNumber = CloneableObject.Numbers.One;

            // Act
            var clone = _obj.CopyObject();

            // Assert
            Assert.IsTrue(clone.PublicNumber == CloneableObject.Numbers.One);
        }
        
        [TestMethod]
        public void CopyObject_WithNestedClass_CopiesNestedClassProperties()
        {
            // Arrange
            _obj.PublicNumber = CloneableObject.Numbers.One;
            _advObj.NestedObject = _obj;

            // Act
            var advClone = _advObj.CopyObject();
            var nestedObjClone = advClone.NestedObject;

            // Assert
            Assert.IsTrue(nestedObjClone.PublicNumber == CloneableObject.Numbers.One);
        }

        [TestMethod]
        public void CopyObject_WithNestedCollection_CopiesNestedCollection()
        {
            // Arrange
            var nestedObjCollection = new List<CloneableObject>();

            var nestedObjCollectionObject = new CloneableObject();
            nestedObjCollection.Add(nestedObjCollectionObject);

            _obj.PublicCollection = nestedObjCollection;
            _advObj.NestedObject = _obj;
            
            // Act
            var advClone = _advObj.CopyObject();
            var nestedObjCollectionClone = advClone.NestedObject.PublicCollection;

            // Assert
            Assert.IsTrue(nestedObjCollectionClone != nestedObjCollection);
        }

        [TestMethod]
        public void CopyObject_ReturnsDereferencedClone()
        {
            // Arrange
            _advObj.NestedObject = _obj;

            // Act
            var advClone = _advObj.CopyObject();
            var nestedObjClone = advClone.NestedObject;

            // Assert
            Assert.IsTrue(advClone != _advObj, "");
            Assert.IsTrue(nestedObjClone != _obj, "");
        }

        #endregion
    }
}
