using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Generic.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Framework.Generic.Tests.EntityFramework
{
    /// <summary>
    /// Tests of mock interface provided because it will provide functionality (and is the backbone) 
    /// for the rest of the unit tests in this project & solution.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class MockEfContextTest
    {        
        private MockEfContext _mockContext;

        [TestInitialize]
        public void Initialize()
        {
            _mockContext = new MockEfContext(typeof(TestEntity));
        }

        #region Testing Constructor...

        [TestMethod]
        public void Constructor_AfterInitialization_NotNull()
        {
            // Assert
            Assert.IsNotNull(_mockContext.Object);
        }

        #endregion
        #region Testing GetDbSet...

        [TestMethod]
        public void GetDbSet_ForNonExistingType_ThrowsException()
        {
            // Assert
            Assert.ThrowsException<KeyNotFoundException>(() => _mockContext.GetDbSet<string>());
        }

        [TestMethod]
        public void GetDbSet_ForExistingType_ReturnsDbSet()
        {
            // Arrange 
            _mockContext.AddEntityDirect(new TestEntity(999));

            // Act
            var dbSet = _mockContext.GetDbSet<TestEntity>();

            // Assert
            Assert.IsNotNull(dbSet, "");
            Assert.IsTrue(dbSet.Count() == 1, "");
        }

        #endregion
        #region Testing AddEntityDirect...

        [TestMethod]
        public void AddEntityDirect_WithValidEntity_AddsEntityToCorrectDbSet()
        {
            // Arrange
            var entity1 = new TestEntity(111);

            // Act
            _mockContext.AddEntityDirect(entity1);
            var dbset = _mockContext.GetDbSet<TestEntity>();

            // Assert
            Assert.IsTrue(dbset.Find(entity1) != null, "Entity1 should've been added to this dbset.");
        }

        #endregion
        #region Testing Get...

        [TestMethod]
        public void Get_ReturnsTestEntities()
        {
            // Arrange
            _mockContext.AddEntityDirect(new TestEntity(999));

            // Act
            var data = _mockContext.Object.Get<TestEntity>();

            // Assert
            Assert.IsTrue(data.Count() == 1, "The number of data elements should equal to the mock elements that were used to load the context.");
        }

        [TestMethod]
        public void Get_ReturnsNull_IfDatasetDoesntContainDataType()
        {
            // Arrange
            _mockContext.AddEntityDirect(new TestEntity(999));

            // Act
            var data = _mockContext.Object.Get<String>();

            // Assert
            Assert.IsNull(data, "Get should return null when fetching a datatype that doesn't exist in the entities.");
        }

        #endregion
        #region Testing SetEntityState...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SetEntityState_NullEntity_ThrowsException()
        {
            // Act
            _mockContext.Object.SetEntityState<TestEntity>(null, EntityState.Added);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SetEntityState_InvalidEntity_ThrowsException()
        {
            // Arrange
            var entity = new TestEntity(999);

            // Act
            _mockContext.Object.SetEntityState(entity, EntityState.Added);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SetEntityState_VirtualEntity_ThrowsException()
        {
            // Arrange            
            var entity = new TestEntity(999);
            entity.IsVirtual = true;

            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.SetEntityState(entity, EntityState.Added);
        }

        [TestMethod]
        public void SetEntityState_ValidEntity_ChangesEntityState()
        {
            // Arrange            
            var entity = new TestEntity(999);
            entity.IsVirtual = false;

            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.SetEntityState(entity, EntityState.Modified);
            var state = entity.State;

            // Assert
            Assert.IsTrue(state == EntityState.Modified, "The entity state should return as modified since that's what it was updated to.");
        }

        #endregion
        #region Testing GetEntityState...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetEntityState_NullEntity_ThrowsException()
        {
            // Act
            _mockContext.Object.GetEntityState<TestEntity>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetEntityState_InvalidEntity_ThrowsException()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            _mockContext.Object.GetEntityState(entity);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetEntityState_VirtualEntity_ThrowsException()
        {
            // Arrange
            var entity = new TestEntity();
            entity.IsVirtual = true;
            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.GetEntityState(entity);
        }

        [TestMethod]
        public void GetEntityState_ValidEntity_ReturnsEntityState()
        {
            // Arrange
            var entity = new TestEntity();
            entity.IsVirtual = false;
            entity.State = EntityState.Modified;
            _mockContext.AddEntityDirect(entity);

            // Act
            var state = _mockContext.Object.GetEntityState(entity);

            // Assert
            Assert.IsTrue(state == EntityState.Modified, "The entity state should return as modified since that's what it was initialized as.");
        }

        #endregion
        #region Testing Create...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Create_NullEntity_ThrowsException()
        {
            // Act
            _mockContext.Object.Add<TestEntity>(null);
        }

        [TestMethod]
        public void Create_ValidEntity_InsertsEntityIntoDbSet()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);

            var entitiesContainsNewEntity = _mockContext.GetDbSet<TestEntity>().Any(e => e.TestId == entityToAdd.TestId);

            // Assert
            Assert.IsTrue(entitiesContainsNewEntity, "The entity should've been added to the entities dbset.");
        }

        [TestMethod]
        public void Create_ValidEntity_InsertsEntityIntoDbSetAsVirtual()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);

            // Assert
            Assert.IsTrue(entityToAdd.IsVirtual, "The entity should've been added to the entities dbset as a virtual entity.");
        }

        [TestMethod]
        public void Create_ValidEntity_InsertsVirtualEntityWithAddedState()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);

            // Assert
            Assert.IsTrue(entityToAdd.State == EntityState.Added, "The entity should've been added to the entities dbset with 'Added' state.");
        }

        #endregion
        #region Testing Update...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Update_NullEntity_ThrowsException()
        {
            // Act
            _mockContext.Object.Update<TestEntity>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Update_NonExistingEntity_ThrowsException()
        {
            // Arrange
            var entityToUpdate = new TestEntity(999);

            // Act
            _mockContext.Object.Update(entityToUpdate);
        }

        [TestMethod]
        public void Update_ValidEntity_UpdatesCurrentValuesForEntity()
        {
            // Arrange
            var entity = new TestEntity(111);
            _mockContext.AddEntityDirect(entity);

            // Act
            entity.CurrentValue = 999;
            _mockContext.Object.Update(entity);

            // Assert
            Assert.IsTrue(entity.CurrentValue == 999, "Current value was updated so it should have updated.");
        }

        [TestMethod]
        public void Update_ValidEntity_UpdatesStateToModified()
        {
            // Arrange
            var entity = new TestEntity(111);
            _mockContext.AddEntityDirect(entity);

            // Act
            entity.CurrentValue = 999;
            _mockContext.Object.Update(entity);

            // Assert
            Assert.IsTrue(entity.State == EntityState.Modified, "Any entity that is updated should have a 'Modified' state.");
        }

        #endregion
        #region Testing Delete...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Delete_NullEntity_ThrowsException()
        {
            // Act
            _mockContext.Object.Delete<TestEntity>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Delete_NonExistingEntity_ThrowsException()
        {
            // Arrange
            var entityToDelete = new TestEntity(999);

            // Act
            _mockContext.Object.Delete(entityToDelete);
        }

        [TestMethod]
        public void Delete_ValidEntity_UpdatesStateToDeleted()
        {
            // Arrange
            var entity = new TestEntity(999);
            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.Delete(entity);

            // Assert
            Assert.IsTrue(entity.State == EntityState.Deleted, "Entity state should be marked as 'Deleted' until the savechanges method is called.");
        }

        #endregion
        #region Testing SaveChanges...

        [TestMethod]
        public void SaveChanges_AfterInsertedEntity_StoresAsNonVirtual()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);
            _mockContext.Object.SaveChanges();

            // Assert
            Assert.IsFalse(entityToAdd.IsVirtual, "When changes are saved, the entity should no longer be virtual.");
        }

        [TestMethod]
        public void SaveChanges_AfterInsertedEntity_UpdatesStoredValueToCurrentValue()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);
            entityToAdd.CurrentValue = 111;

            // Act
            _mockContext.Object.Add(entityToAdd);
            _mockContext.Object.SaveChanges();

            // Assert
            Assert.IsTrue(entityToAdd.StoredValue == 111, "Save changes should update the stored value of a newly inserted item.");
        }

        [TestMethod]
        public void SaveChanges_AfterUpdatedEntity_UpdatesStoredValueToCurrentValue()
        {
            // Arrange
            var entity = new TestEntity(999);
            _mockContext.AddEntityDirect(entity);

            // Act
            entity.CurrentValue = 111;
            _mockContext.Object.Update(entity);
            _mockContext.Object.SaveChanges();

            // Assert
            Assert.IsTrue(entity.StoredValue == 111, "When changes are saved, the stored value should be updated to the editted current value.");
            Assert.IsTrue(entity.CurrentValue == 111, "When changes are saved, the stored value should be updated to the editted current value.");
        }

        [TestMethod]
        public void SaveChanges_RemovesDeletedEntities()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.State = EntityState.Deleted;
            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.SaveChanges();

            var deletedEntity = _mockContext.GetDbSet<TestEntity>().FirstOrDefault(e => e.TestId == entity.TestId);

            // Assert
            Assert.IsNull(deletedEntity, "When changes are saved, the entity should have been removed from the set.");
        }

        [TestMethod]
        public void SaveChanges_AfterSuccessfulSave_Returns1()
        {
            // Act
            var saveVal = _mockContext.Object.SaveChanges();

            // Assert
            Assert.IsTrue(saveVal == 1);
        }

        #endregion
        #region Testing SaveChangesAsync...

        [TestMethod]
        public async Task SaveChangesAsync_AfterInsertedEntity_StoresAsNonVirtual()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);
            await _mockContext.Object.SaveChangesAsync();

            // Assert
            Assert.IsFalse(entityToAdd.IsVirtual, "When changes are saved, the entity should no longer be virtual.");
        }

        [TestMethod]
        public async Task SaveChangesAsync_AfterInsertedEntity_UpdatesStoredValueToCurrentValue()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);
            entityToAdd.CurrentValue = 111;

            // Act
            _mockContext.Object.Add(entityToAdd);
            await _mockContext.Object.SaveChangesAsync();

            // Assert
            Assert.IsTrue(entityToAdd.StoredValue == 111, "Save changes should update the stored value of a newly inserted item.");
        }

        [TestMethod]
        public async Task SaveChangesAsync_AfterUpdatedEntity_UpdatesStoredValueToCurrentValue()
        {
            // Arrange
            var entity = new TestEntity(999);
            _mockContext.AddEntityDirect(entity);

            // Act
            entity.CurrentValue = 111;
            _mockContext.Object.Update(entity);
            await _mockContext.Object.SaveChangesAsync();

            // Assert
            Assert.IsTrue(entity.StoredValue == 111, "When changes are saved, the stored value should be updated to the editted current value.");
            Assert.IsTrue(entity.CurrentValue == 111, "When changes are saved, the stored value should be updated to the editted current value.");
        }

        [TestMethod]
        public async Task SaveChangesAsync_RemovesDeletedEntities()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.State = EntityState.Deleted;
            _mockContext.AddEntityDirect(entity);

            // Act
            await _mockContext.Object.SaveChangesAsync();

            var deletedEntity = _mockContext.GetDbSet<TestEntity>().FirstOrDefault(e => e.TestId == entity.TestId);

            // Assert
            Assert.IsNull(deletedEntity, "When changes are saved, the entity should have been removed from the set.");
        }

        [TestMethod]
        public async Task SaveChangesAsync_AfterSuccessfulSave_Returns1()
        {
            // Act
            var val = await _mockContext.Object.SaveChangesAsync();
            
            // Assert
            Assert.IsTrue(val == 1);
        }

        #endregion
        #region Testing RevertChanges...

        [TestMethod]
        public void RevertChanges_RevertsValueChanges()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.CurrentValue = 111;
            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.RevertChanges();

            // Assert
            Assert.IsTrue(entity.CurrentValue == 999, "When changes are reverted, the current value should be updated to the previously stored value.");
        }

        [TestMethod]
        public void RevertChanges_RevertsEntityStateChanges()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.State = EntityState.Modified;
            _mockContext.AddEntityDirect(entity);

            // Act
            _mockContext.Object.RevertChanges();

            // Assert
            Assert.IsTrue(entity.State == EntityState.Unchanged, "After changes have been reverted, the entity state is automatically updated to unchanged.");
        }

        [TestMethod]
        public void RevertChanges_RemovesVirtualEntities()
        {
            // Arrange
            _mockContext.AddEntityDirect(new TestEntity(111) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(112) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(113) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(114) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(115) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(116) { IsVirtual = true });
            
            // Act
            _mockContext.Object.RevertChanges();

            var containsEntities = _mockContext.GetDbSet<TestEntity>().Any();

            // Assert
            Assert.IsFalse(containsEntities, "The collection shouldn't contain any entities because all virtual entities are removed when changes are reverted.");
        }

        [TestMethod]
        public void RevertChanges_AfterInsertedEntity_RemovesEntity()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);
            _mockContext.Object.RevertChanges();

            var revertedEntity = _mockContext.GetDbSet<TestEntity>().FirstOrDefault(e => e.TestId == entityToAdd.TestId);

            // Assert
            Assert.IsNull(revertedEntity, "The inserted entity should have been removed completely when changes were reverted.");
        }

        [TestMethod]
        public void RevertChanges_AfterDeletedEntity_DoesNotDeleteEntity()
        {
            // Arrange
            var entityToDelete = new TestEntity(999);
            _mockContext.AddEntityDirect(entityToDelete);

            // Act
            _mockContext.Object.Delete(entityToDelete);
            _mockContext.Object.RevertChanges();

            var revertedEntity = _mockContext.GetDbSet<TestEntity>().FirstOrDefault(e => e.TestId == entityToDelete.TestId);

            // Assert
            Assert.IsNotNull(revertedEntity, "The entity should exist because changes were reverted.");
        }

        [TestMethod]
        public void RevertChanges_AfterDeletedEntity_ChangesStateToUnchanged()
        {
            // Arrange
            var entityToDelete = new TestEntity(999);
            _mockContext.AddEntityDirect(entityToDelete);

            // Act
            _mockContext.Object.Delete(entityToDelete);
            _mockContext.Object.RevertChanges();

            // Assert
            Assert.IsTrue(entityToDelete.State == EntityState.Unchanged, "The entity state should have been updated to 'Unchanged' after changes were reverted.");
        }

        #endregion
        #region Testing RevertChangesAsync...

        [TestMethod]
        public async Task RevertChangesAsync_RevertsValueChanges()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.CurrentValue = 111;
            _mockContext.AddEntityDirect(entity);
            
            // Act
            await _mockContext.Object.RevertChangesAsync();

            // Assert
            Assert.IsTrue(entity.CurrentValue == 999, "When changes are reverted, the current value should be updated to the previously stored value.");
        }

        [TestMethod]
        public async Task RevertChangesAsync_RevertsEntityStateChanges()
        {
            // Arrange
            var entity = new TestEntity(999);
            entity.State = EntityState.Modified;
            _mockContext.AddEntityDirect(entity);
            
            // Act
            await _mockContext.Object.RevertChangesAsync();

            // Assert
            Assert.IsTrue(entity.State == EntityState.Unchanged, "After changes have been reverted, the entity state is automatically updated to unchanged.");
        }

        [TestMethod]
        public async Task RevertChangesAsync_RemovesVirtualEntities()
        {
            // Arrange
            _mockContext.AddEntityDirect(new TestEntity(111) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(112) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(113) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(114) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(115) { IsVirtual = true });
            _mockContext.AddEntityDirect(new TestEntity(116) { IsVirtual = true });
            
            // Act
            await _mockContext.Object.RevertChangesAsync();

            var containsEntities = _mockContext.GetDbSet<TestEntity>().Any();

            // Assert
            Assert.IsFalse(containsEntities, "The collection shouldn't contain any entities because all virtual entities are removed when changes are reverted.");
        }

        [TestMethod]
        public async Task RevertChangesAsync_AfterInsertedEntity_RemovesEntity()
        {
            // Arrange
            var entityToAdd = new TestEntity(999);

            // Act
            _mockContext.Object.Add(entityToAdd);
            await _mockContext.Object.RevertChangesAsync();

            var revertedEntity = _mockContext.GetDbSet<TestEntity>().FirstOrDefault(e => e.TestId == entityToAdd.TestId);

            // Assert
            Assert.IsNull(revertedEntity, "The inserted entity should have been removed completely when changes were reverted.");
        }

        [TestMethod]
        public async Task RevertChangesAsync_AfterDeletedEntity_DoesNotDeleteEntity()
        {
            // Arrange
            var entityToDelete = new TestEntity(999);
            _mockContext.AddEntityDirect(entityToDelete);

            // Act
            _mockContext.Object.Delete(entityToDelete);
            await _mockContext.Object.RevertChangesAsync();

            var revertedEntity = _mockContext.GetDbSet<TestEntity>().FirstOrDefault(e => e.TestId == entityToDelete.TestId);

            // Assert
            Assert.IsNotNull(revertedEntity, "The entity should exist because changes were reverted.");
        }

        [TestMethod]
        public async Task RevertChangesAsync_AfterDeletedEntity_ChangesStateToUnchanged()
        {
            // Arrange
            var entityToDelete = new TestEntity(999);
            _mockContext.AddEntityDirect(entityToDelete);

            // Act
            _mockContext.Object.Delete(entityToDelete);
            await _mockContext.Object.RevertChangesAsync();

            // Assert
            Assert.IsTrue(entityToDelete.State == EntityState.Unchanged, "The entity state should have been updated to 'Unchanged' after changes were reverted.");
        }

        #endregion
        #region Testing Dispose...

        [TestMethod]
        public void Dispose_RemovedDbSets()
        {
            // Arrange
            _mockContext = new MockEfContext(typeof(TestEntity));

            // Act
            _mockContext.Object.Dispose();
            
            // Assert
            Assert.ThrowsException<NullReferenceException>(() => _mockContext.GetDbSet<TestEntity>(), "The dbset should not exist once the context has been disposed.");
        }
        
        #endregion
    }
}
