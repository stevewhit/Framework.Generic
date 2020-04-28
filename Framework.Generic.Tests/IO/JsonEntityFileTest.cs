using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Framework.Generic.IO;
using Framework.Generic.Tests.Builders;
using Framework.Generic.Tests.Builders.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Framework.Generic.Tests.IO
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class JsonEntityFileTest
    {
        private const string _fakeFilePath = @"FakePath:\FakeDirectory\JsonEntityFileTests.txt";
        private IFileSystem _mockFileSystem;
        private JsonEntityFile<TestSerializedJsonEntity> _entityFile;

        [TestInitialize]
        public void Initialize()
        {
            _mockFileSystem = new MockFileSystem().Object;
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(_fakeFilePath, _mockFileSystem);
        }

        #region Testing JsonEntityFile(string filePath)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JsonEntityFileString_WithNullFilePath_ThrowsException()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JsonEntityFileString_WithEmptyFilePath_ThrowsException()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(string.Empty);
        }

        [TestMethod]
        public void JsonEntityFileString_WithValidFilePath_IsNotNull()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(_fakeFilePath);

            // Assert
            Assert.IsNotNull(_entityFile);
        }

        #endregion
        #region Testing JsonEntityFile(string filePath, IFileSystem fileSystem)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JsonEntityFileStringFileSystem_WithNullFilePath_ThrowsException()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(null, _mockFileSystem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JsonEntityFileStringFileSystem_WithEmptyFilePath_ThrowsException()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(string.Empty, _mockFileSystem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JsonEntityFileStringFileSystem_WithNullFileSystem_ThrowsException()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(_fakeFilePath, null);
        }

        [TestMethod]
        public void JsonEntityFileStringFileSystem_WithValidArgs_IsNotNull()
        {
            // Act
            _entityFile = new JsonEntityFile<TestSerializedJsonEntity>(_fakeFilePath, _mockFileSystem);

            // Assert
            Assert.IsNotNull(_entityFile);
        }

        #endregion
        #region Testing FEntity GetEntity()...

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetEntity_WithNonExistingFile_ThrowsException()
        {
            // Act
            _entityFile.GetEntity();
        }

        [TestMethod]
        public void GetEntity_WithExistingFile_ReturnsEntity()
        {
            // Arrange
            var testObj = new TestSerializedJsonEntity(111, "fakeName");
            _entityFile.WriteEntity(testObj);

            // Act
            var returnedEntity = _entityFile.GetEntity();

            // Assert
            Assert.IsNotNull(returnedEntity);
            Assert.IsTrue(testObj.Id == returnedEntity.Id);
        }

        #endregion
        #region Testing WriteEntity(FEntity entity)

        [TestMethod]
        public void WriteEntity_WithNonExistingFile_WritesToNewFile()
        {
            // Arrange
            var testObj = new TestSerializedJsonEntity(111, "fakeName");

            // Act
            _entityFile.WriteEntity(testObj);

            var storedEntity = _entityFile.GetEntity();

            // Assert
            Assert.IsNotNull(storedEntity);
            Assert.IsTrue(testObj.Id == storedEntity.Id);
        }

        [TestMethod]
        public void WriteEntity_WithExistingFile_UpdatesFile()
        {
            // Arrange
            var firstValue = 111;
            var testObj = new TestSerializedJsonEntity(firstValue, "fakeName");
            _entityFile.WriteEntity(testObj);

            var updatedValue = 222;
            testObj.Value = updatedValue;

            // Act
            _entityFile.WriteEntity(testObj);

            var storedEntity = _entityFile.GetEntity();

            // Assert
            Assert.IsNotNull(storedEntity);
            Assert.IsTrue(storedEntity.Value == updatedValue);
        }

        #endregion
    }
}
