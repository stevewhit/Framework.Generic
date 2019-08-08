using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Framework.Generic.Utility;
using Framework.Generic.Tests.Builders;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Framework.Generic.Tests.Builders.Objects;
using System.Linq;

namespace Framework.Generic.Tests.Utility
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EnumerableUtilsTest
    {
        private ICollection<TestEntity> _entities;

        [TestInitialize]
        public void Initialize()
        {
            _entities = new Collection<TestEntity>();
        }

        #region Testing IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ForEach_WithNullSource_ThrowsException()
        {
            // Arrange
            _entities = null;

            // Act
            _entities.ForEach(ent => ent.CurrentValue = -1);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ForEach_WithNullAction_ThrowsException()
        {
            // Arrange
            _entities.Add(new TestEntity(111));

            // Act
            _entities.ForEach(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ForEach_WithNullEntity_ThrowsException()
        {
            // Arrange
            _entities.Add(null);

            // Act
            _entities.ForEach(ent => ent.CurrentValue = -1);
        }

        [TestMethod]
        public void ForEach_ValidEntitiesAndAction_PerformsActionToEntities()
        {
            // Arrange
            var entity1 = new TestEntity(111);
            _entities.Add(entity1);
            var entity2 = new TestEntity(222);
            _entities.Add(entity2);

            // Act
            _entities.ForEach(ent => ent.CurrentValue = -1);

            // Assert
            Assert.IsTrue(entity1.CurrentValue == -1, "Entity should've been updated when the action was applied in the foreach method.");
            Assert.IsTrue(entity2.CurrentValue == -1, "Entity should've been updated when the action was applied in the foreach method.");
        }

        [TestMethod]
        public void ForEach_WithValidEntities_ReturnsSourceEnumerable()
        {
            // Arrange
            var entity1 = new TestEntity(111);
            _entities.Add(entity1);
            var entity2 = new TestEntity(222);
            _entities.Add(entity2);

            // Act
            var sourceCopy = _entities.ForEach(ent => ent.CurrentValue = -1);

            // Assert
            Assert.IsTrue(sourceCopy == _entities, "The method should have returned the updated enumerable without making a clone.");
        }

        #endregion
        #region IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Shuffle_WithNullSource_ThrowsException()
        {
            // Arrange 
            List<int> source = null;

            // Act
            source.Shuffle();
        }

        [TestMethod]
        public void Shuffle_WithEmptySource_ReturnsSource()
        {
            // Arrange 
            List<int> source = new List<int>();

            // Act
            source.Shuffle();

            // Assert
            Assert.IsTrue(source.Count == 0);
        }

        [TestMethod]
        public void Shuffle_WithPopulatedSource_ReturnsShuffledSource()
        {
            // Arrange 
            List<int> source = new List<int>();
            int sourceCount = 100;

            for (int i = 0; i < sourceCount; i++)
                source.Add(i);

            // Act
            source.Shuffle();

            // Assert
            Assert.IsTrue(source.Count == sourceCount);
            Assert.IsTrue(source[0] != 0 || source[1] != 1, "The source should've been shuffled enough where atleast 2 items shouldn't be in the same location.");        
        }

        #endregion
        #region void Dispose(this IEnumerable<IDisposable> disposableObjs)...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Dispose_WithNullObjects_ThrowsException()
        {
            // Arrange 
            List<DisposableObject> disposableObjs = null;

            // Act
            disposableObjs.Dispose();
        }

        [TestMethod]
        public void Dispose_WithEmptyObjects_DoesNothing()
        {
            // Arrange 
            var disposableObjs = new List<DisposableObject>();

            // Act
            disposableObjs.Dispose();
        }

        [TestMethod]
        public void Dispose_WithPopulatedObjects_DisposesAllObjects()
        {
            // Arrange 
            var disposableObjs = new List<DisposableObject>();
            int sourceCount = 100;

            for (int i = 0; i < sourceCount; i++)
                disposableObjs.Add(new DisposableObject());

            // Act
            disposableObjs.Dispose();

            // Assert
            Assert.IsTrue(disposableObjs.Count(o => o.IsDisposed == true) == sourceCount);
        }

        #endregion
    }
}
