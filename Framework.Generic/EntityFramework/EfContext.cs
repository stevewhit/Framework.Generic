using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Generic.EntityFramework
{
    public interface IEfContext : IDisposable
    {
        IDbSet<TEntity> Get<TEntity>() where TEntity : class;
        void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class;
        EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class;
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void RevertChanges();
        Task RevertChangesAsync();
    }
    
    [ExcludeFromCodeCoverage]
    public class EfContext : IEfContext
    {
        private DbContext _context;
        private bool _isDisposed = false;
        
        public EfContext(DbContext context)
        {
            _context = context;
        }

        #region IEfContext Support

        /// <summary>
        /// Returns the DbSet entities for the designated type.
        /// </summary>
        public virtual IDbSet<TEntity> Get<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// Sets the state for the entity object.
        /// </summary>
        public virtual void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class
        {
            _context.Entry(entity).State = state;
        }

        /// <summary>
        /// Returns the state for the entity object.
        /// </summary>
        public virtual EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class
        {
            return _context.Entry(entity).State;
        }

        /// <summary>
        /// Inserts a new entity object into the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
            SetEntityState(entity, EntityState.Added);
        }

        /// <summary>
        /// Inserts new entity objects into the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _context.Set<TEntity>().AddRange(entities);

            foreach(var entity in entities)
                SetEntityState(entity, EntityState.Added);
        }

        /// <summary>
        /// Updates the existing entity object in the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Attach(entity);
            SetEntityState(entity, EntityState.Modified);
        }

        /// <summary>
        /// Deletes the existing entity object from the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
            SetEntityState(entity, EntityState.Deleted);
        }

        /// <summary>
        /// Saves any changes that have been made to the data model.
        /// </summary>
        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Saves any changes that have been made to the data model asynchronously.
        /// </summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        
        /// <summary>
        /// Reverts any unsaved changes that have been made to the data model.
        /// </summary>
        public virtual void RevertChanges()
        {
            var changedEntries = _context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                    case EntityState.Detached:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Reverts any unsaved changes that have been made to the data model asynchronously.
        /// </summary>
        public virtual async Task RevertChangesAsync()
        {
            await Task.Run(() => RevertChanges());
        }

        /// <summary>
        /// Disposes this object and properly cleans up resources. 
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _context = null;
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Disposes this object and properly cleans up resources. 
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
