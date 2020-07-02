using Framework.Generic.EntityFramework;
using Moq;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Framework.Generic.Tests.Builders
{
    public class MockEfContext : Mock<IEfContext>
    {
        /// <summary>
        /// The entity dbsets that hold the entities for this context. 
        /// </summary>
        private IDictionary<Type, object> _dbSets;

        public MockEfContext(Type supportedEntityType) : this(new List<Type>() { supportedEntityType }) { }
        public MockEfContext(IEnumerable<Type> supportedEntityTypes)
        {
            InitializeDbSets(supportedEntityTypes);

            // Setup all mock methods.
            SetupGet().SetupSetEntityState().SetupGetEntityState().SetupAdd().SetupAddRange().SetupUpdate().SetupDelete().SetupSaveChanges().SetupSaveChangesAsync().SetupRevertChanges().SetupRevertChangesAsync().SetupDispose();
        }
        
        /// <summary>
        /// Returns the stored dbset matching the generic type.
        /// </summary>
        public IDbSet<T> GetDbSet<T>() where T : class
        {
            return _dbSets[typeof(T)] as IDbSet<T>;
        }
                
        /// <summary>
        /// Directly adds the supplied entity to the appropriate entity dbset and
        /// by-passes the "insert" method.
        /// </summary>
        public void AddEntityDirect<T>(T entity) where T : class, ITestEntity
        {
            GetDbSet<T>().Add(entity);
        }

        /// <summary>
        /// Finds the method (using its name) and invokes it.
        /// </summary>
        private object CallMethodForType(string methodName, Type genericType, BindingFlags bindingAttrs = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var method = this.GetType().GetMethod(methodName, bindingAttrs).MakeGenericMethod(genericType);
            return method.Invoke(this, new object[] { });
        }

        /// <summary>
        /// Finds the method (using its name) and invokes it with each of the unique entity types.
        /// </summary>
        private MockEfContext CallMethodForAllTypes(string methodName, BindingFlags bindingAttrs = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            foreach (var entityType in _dbSets.Keys)
            {
                CallMethodForType(methodName, entityType, bindingAttrs);
            }

            return this;
        }

        #region IntializeDbSets
        /// <summary>
        /// Creates and stores new IDbSets for each of the supported entity types.
        /// </summary>
        private void InitializeDbSets(IEnumerable<Type> supportedEntityTypes, BindingFlags bindingAttrs = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            _dbSets = new Dictionary<Type, object>();

            foreach (var entityType in supportedEntityTypes.Distinct())
            {
                CallMethodForType("AddDbSet", entityType, bindingAttrs);
            }
        }

        /// <summary>
        /// Adds a new IDbSet to the collection using the generic type.
        /// </summary>
        private void AddDbSet<T>() where T : class, ITestEntity
        {
            _dbSets.Add(typeof(T), (object)(new MockDbSet<T>().Object));
        }
        #endregion
        #region SetupGet
        public MockEfContext SetupGet()
        {
            return CallMethodForAllTypes("SetupGet");
        }

        private void SetupGet<T>() where T : class, ITestEntity
        {
            Setup(c => c.Get<T>()).Returns(() =>
            {
                return GetDbSet<T>();
            });
        }
        #endregion
        #region SetupSetEntityState
        public MockEfContext SetupSetEntityState()
        {
            return CallMethodForAllTypes("SetupSetEntityState");
        }

        private void SetupSetEntityState<T>() where T : class, ITestEntity
        {
            Setup(c => c.SetEntityState<T>(It.IsAny<T>(), It.IsAny<EntityState>()))
                .Callback((T entity, EntityState state) =>
                {
                    var entityToFind = !entity.IsVirtual ? entity : null;
                    GetDbSet<T>().Find(entityToFind).State = state;
                });
        }
        #endregion
        #region SetupGetEntityState
        public MockEfContext SetupGetEntityState()
        {
            return CallMethodForAllTypes("SetupGetEntityState");
        }

        private void SetupGetEntityState<T>() where T : class, ITestEntity
        {
            Setup(c => c.GetEntityState<T>(It.IsAny<T>()))
                .Returns((T entity) =>
                {
                    var entityToFind = !entity.IsVirtual ? entity : null;

                    return GetDbSet<T>().Find(entityToFind).State;
                });
        }
        #endregion
        #region SetupAdd
        public MockEfContext SetupAdd()
        {
            return CallMethodForAllTypes("SetupAdd");
        }

        private void SetupAdd<T>() where T : class, ITestEntity
        {
            Setup(c => c.Add<T>(It.IsAny<T>()))
                .Callback((T entity) =>
                {
                    entity.IsVirtual = true;
                    entity.State = EntityState.Added;

                    GetDbSet<T>().Add(entity);
                });
        }
        #endregion
        #region SetupAddRange
        public MockEfContext SetupAddRange()
        {
            return CallMethodForAllTypes("SetupAddRange");
        }

        private void SetupAddRange<T>() where T : class, ITestEntity
        {
            Setup(c => c.AddRange<T>(It.IsAny<IEnumerable<T>>()))
                .Callback((IEnumerable<T> entities) =>
                {
                    foreach (var entity in entities)
                    {
                        entity.IsVirtual = true;
                        entity.State = EntityState.Added;
                        GetDbSet<T>().Add(entity);
                    }                    
                });
        }
        #endregion
        #region SetupUpdate
        public MockEfContext SetupUpdate()
        {
            return CallMethodForAllTypes("SetupUpdate");
        }

        private void SetupUpdate<T>() where T : class, ITestEntity
        {
            Setup(c => c.Update<T>(It.IsAny<T>()))
                .Callback((T entity) =>
                {
                    var existingEntity = GetDbSet<T>().Find(entity);

                    // Don't manipulate the stored value.
                    existingEntity.CurrentValue = entity.CurrentValue;
                    existingEntity.State = EntityState.Modified;
                });
        }
        #endregion
        #region SetupDelete
        public MockEfContext SetupDelete()
        {
            return CallMethodForAllTypes("SetupDelete");
        }

        private void SetupDelete<T>() where T : class, ITestEntity
        {
            Setup(c => c.Delete<T>(It.IsAny<T>()))
                .Callback((T entity) =>
                {
                    var existingEntity = GetDbSet<T>().Find(entity);
                    existingEntity.State = EntityState.Deleted;
                });
        }
        #endregion
        #region SetupSaveChanges
        public MockEfContext SetupSaveChanges()
        {
            Setup(c => c.SaveChanges())
                .Returns(() =>
                {
                    CallMethodForAllTypes("SaveChanges");
                    return 1;
                });

            return this;
        }

        private void SaveChanges<T>() where T : class, ITestEntity
        {
            var dbSet = GetDbSet<T>();

            // Remove deleted entities
            var deletedEntities = dbSet.Where(e => e.State == EntityState.Deleted).ToList();
            foreach (var entity in deletedEntities)
            {
                dbSet.Remove(entity);
            }

            // Save updated entities
            foreach (var entity in dbSet)
            {
                entity.StoredValue = entity.CurrentValue;
                entity.State = EntityState.Unchanged;
                entity.IsVirtual = false;
            }
        }
        #endregion
        #region SetupSaveChangesAsync
        public MockEfContext SetupSaveChangesAsync()
        {
            Setup(c => c.SaveChangesAsync())
                .Returns(() =>
                {
                    return Task.Run<int>(() => { CallMethodForAllTypes("SaveChanges"); return 1; });
                });

            return this;
        }
        #endregion
        #region SetupRevertChanges
        public MockEfContext SetupRevertChanges()
        {
            Setup(c => c.RevertChanges())
                .Callback(() =>
                {
                    CallMethodForAllTypes("RevertChanges");
                });

            return this;
        }

        private void RevertChanges<T>() where T : class, ITestEntity
        {
            var dbSet = GetDbSet<T>();

            // Remove virtual entities
            var virtualEntities = dbSet.Where(e => e.IsVirtual).ToList();
            foreach (var entity in virtualEntities)
            {
                dbSet.Remove(entity);
            }

            // Revert updated entities
            foreach (var entity in dbSet)
            {
                entity.CurrentValue = entity.StoredValue;
                entity.State = EntityState.Unchanged;
            }
        }
        #endregion
        #region SetupRevertChangesAsync
        public MockEfContext SetupRevertChangesAsync()
        {
            Setup(c => c.RevertChangesAsync())
                .Returns(() =>
                {
                    return Task.Run(() => { CallMethodForAllTypes("RevertChanges"); });
                });

            return this;
        }
        #endregion
        #region SetupDispose
        public MockEfContext SetupDispose()
        {
            Setup(c => c.Dispose())
                .Callback(() =>
                {
                    _dbSets = null;
                });

            return this;
        }
        #endregion
    }
}
