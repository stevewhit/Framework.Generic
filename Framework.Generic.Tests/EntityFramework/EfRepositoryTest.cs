using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Generic.EntityFramework;
using Framework.Generic.Tests.Builders;
using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Generic.Tests.EntityFramework
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EfRepositoryTest
    {
        private EfRepository<TestEntity> _repository;

        [TestInitialize]
        public void Initialize()
        {
            var _mockContext = new MockEfContext(typeof(TestEntity)).Object;
            _repository = new EfRepository<TestEntity>(_mockContext);
        }

        #region Testing Constructor...

        [TestMethod]
        public void Constructor_AfterInitialization_NotNull()
        {
            // Assert
            Assert.IsNotNull(_repository, "After initialization the repository should not be null.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullContext_ThrowsException()
        {
            // Act
            var invalid = new EfRepository<TestEntity>(null);
        }

        #endregion
        #region Testing GetEntities...

        [TestMethod]
        public void GetEntities_WithNoData_ReturnsEmpty()
        {
            // Act
            var data = _repository.GetEntities();

            // Assert
            Assert.IsNotNull(data, "The data should not be null with loaded entities.");
            Assert.IsTrue(data.Count() == 0, "The repository should have no loaded entities.");
        }

        [TestMethod]
        public void GetEntities_WithValidData_ReturnsData()
        {
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);
            
            // Act
            var data = _repository.GetEntities();

            // Assert
            Assert.IsNotNull(data, "The data should not be null with loaded entities.");
            Assert.IsTrue(data.Count() == 1, "The repository should have loaded entities.");
        }

        #endregion
        #region Testing Add...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Add_WithNullEntity_ThrowsException()
        {
            // Act
            _repository.Add(null);
        }

        [TestMethod]
        public void Add_WithValidEntity_AddsVirtualEntity()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _repository.Add(entityToAdd);

            var entitiesContainsNewEntity = _repository.GetEntities().Any(e => e.TestId == entityToAdd.TestId);

            // Assert
            Assert.IsTrue(entitiesContainsNewEntity, "The entity should've been added to the entities dbset.");
        }

        #endregion
        #region Testing Update...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Update_WithNullEntity_ThrowsException()
        {
            // Act
            _repository.Update(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Update_ToInvalidEntity_ThrowsException()
        {
            // Arrange
            var entity = new TestEntity(999);

            // Act
            _repository.Update(entity);
        }

        [TestMethod]
        public void Update_ExistingEntityWithoutSaving_UpdatesEntityCurrentValue()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            entity.CurrentValue = 111;
            _repository.Update(entity);
            
            // Assert
            Assert.IsTrue(entity.CurrentValue == 111, "Current value was updated so it should have updated.");
        }

        [TestMethod]
        public void Update_ExistingEntityWithoutSaving_UpdatesEntityStateToModified()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            entity.CurrentValue = 111;
            _repository.Update(entity);

            // Assert
            Assert.IsTrue(entity.State == EntityState.Modified, "Any updated entity should have a 'modified' state.");
        }

        #endregion
        #region Testing Delete...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Delete_NullEntity_ThrowsException()
        {
            // Act
            _repository.Delete(entity: null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Delete_NonExistingEntity_ThrowsException()
        {
            // Arrange 
            var entity = new TestEntity(999);

            // Act
            _repository.Delete(entity);
        }

        [TestMethod]
        public void Delete_DetachedEntity_ReAttachesAndDeletes()
        {
            // Arrange 
            var entity = new TestEntity(999);
            entity.State = EntityState.Detached;

            _repository.GetEntities().Add(entity);

            // Act
            _repository.Delete(entity);

            // Assert
            Assert.IsTrue(entity.State == EntityState.Deleted, "Entity state should be marked as deleted until the savechanges method is called.");
        }

        [TestMethod]
        public void Delete_ValidEntity_MarksEntityAsDeleted()
        {
            // Arrange 
            var entity = new TestEntity(999)
            {
                State = EntityState.Detached
            };

            _repository.GetEntities().Add(entity);

            // Act
            _repository.Delete(entity);

            // Assert
            Assert.IsTrue(entity.State == EntityState.Deleted, "Entity state should be marked as deleted until the savechanges method is called.");
        }

        #endregion
        #region Testing SaveChanges...

        [TestMethod]
        public void SaveChanges_AfterInsertedEntity_AddsEntity()
        {
            // Arrange            
            var entityToInsert = new TestEntity(999);

            // Act
            _repository.Add(entityToInsert);
            _repository.SaveChanges();

            // Assert
            Assert.IsTrue(entityToInsert.State == EntityState.Unchanged, "Any saved added entity should have an 'unchanged' state.");
        }

        [TestMethod]
        public void SaveChanges_AfterInsertedEntity_UpdatesStoredValue()
        {
            // Arrange
            var entityToInsert = new TestEntity(999);
            entityToInsert.StoredValue = 111;

            // Act
            _repository.Add(entityToInsert);
            _repository.SaveChanges();

            // Assert
            Assert.IsTrue(entityToInsert.StoredValue == 999, "Stored value should have been updated to the 'CurrentValue' when saved.");
        }

        [TestMethod]
        public void SaveChanges_AfterInsertedEntity_MakesEntityNonVirtual()
        {
            // Arrange
            var entityToInsert = new TestEntity(999);
            entityToInsert.StoredValue = 111;

            // Act
            _repository.Add(entityToInsert);
            _repository.SaveChanges();

            // Assert
            Assert.IsTrue(!entityToInsert.IsVirtual, "Inserted entities in the repository should be non-virtual once they are saved.");
        }

        [TestMethod]
        public void SaveChanges_AfterUpdatedEntity_UpdatesStoredValue()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            entity.CurrentValue = 111;

            _repository.Update(entity);
            _repository.SaveChanges();

            // Assert
            Assert.IsTrue(entity.StoredValue == 111, "Stored value should've been updated.");
        }

        [TestMethod]
        public void SaveChanges_AfterUpdatedEntity_UpdatesEntityStateToUnchanged()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            entity.CurrentValue = 111;

            _repository.Update(entity);
            _repository.SaveChanges();

            // Assert
            Assert.IsTrue(entity.State == EntityState.Unchanged, "Any updated entity should have an 'unchanged' state after being saved.");
        }

        [TestMethod]
        public void SaveChanges_AfterDeletedEntity_RemovesEntity()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            _repository.Delete(entity);
            _repository.SaveChanges();

            var repoContainsEntity = _repository.GetEntities().Any(e => e.TestId == entity.TestId);

            // Assert
            Assert.IsFalse(repoContainsEntity, "Entities list should no longer contain an entity with the matching id.");
        }

        #endregion
        #region Testing RevertChanges...

        [TestMethod]
        public void RevertChanges_UpdatedEntity_RevertsValueChanges()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            entity.CurrentValue = 111;
            _repository.Update(entity);
            _repository.RevertChanges();

            // Assert
            Assert.IsTrue(entity.CurrentValue == 999, "When changes are reverted, the stored value should be updated to the previously stored value.");
            Assert.IsTrue(entity.StoredValue == 999, "When changes are reverted, the stored value should be updated to the previously stored value.");
        }

        [TestMethod]
        public void RevertChanges_UpdatedEntity_RevertsEntityStateChange()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            entity.CurrentValue = 111;
            _repository.Update(entity);
            _repository.RevertChanges();

            // Assert
            Assert.IsTrue(entity.State == EntityState.Unchanged, "When changes are reverted, the entity state should be reverted to 'Unchanged'.");
        }

        [TestMethod]
        public void RevertChanges_RevertsEntityStateChanges()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.State = EntityState.Modified;

            _repository.GetEntities().Add(entity);

            // Act
            _repository.RevertChanges();

            // Assert
            Assert.IsTrue(entity.State == EntityState.Unchanged, "When changes are reverted, the entity state should be reverted to 'Unchanged'.");
        }

        [TestMethod]
        public void RevertChanges_AfterInsertedEntity_RemovesEntity()
        {
            // Arrange
            var entity = new TestEntity(999);

            // Act
            _repository.Add(entity);
            _repository.RevertChanges();

            var repoContainsEntity = _repository.GetEntities().Any(e => e.TestId == entity.TestId);

            // Assert
            Assert.IsFalse(repoContainsEntity, "The entity set should not contain the inserted entity after being reverted.");
        }

        [TestMethod]
        public void RevertChanges_AfterDeletedEntity_ReAttachesEntity()
        {
            // Arrange
            var entity = new TestEntity(999);
            _repository.GetEntities().Add(entity);

            // Act
            _repository.Delete(entity);
            _repository.RevertChanges();

            var repoContainsEntity = _repository.GetEntities().Any(e => e.TestId == entity.TestId);

            // Assert
            Assert.IsTrue(repoContainsEntity, "The entity set should contain the deleted entity after being reverted.");
        }

        #endregion
        #region Testing Dispose...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Dispose_AndThenTryToReuseRepository_ThrowsException1()
        {
            // Act
            _repository.Dispose();
            var invalid = _repository.GetEntities();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Dispose_AndThenTryToReuseRepository_ThrowsException2()
        {
            // Act
            _repository.Dispose();
            _repository.SaveChanges();
        }

        #endregion
    }
}
