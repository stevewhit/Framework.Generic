using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;

namespace Framework.Generic.EntityFramework
{
    public interface IEfRepository<TEntity> : IDisposable where TEntity : class
    {
        IDbSet<TEntity> GetEntities();
        Task<IEnumerable<TEntity>> GetEntitiesAsync();
        Task<IEnumerable<TEntity>> GetEntitiesWhereAsync(Expression<Func<TEntity, bool>> expression);

        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        void SaveChanges();
        Task SaveChangesAsync();
        void RevertChanges();
        Task RevertChangesAsync();
    }

    public class EfRepository<TEntity> : IEfRepository<TEntity> where TEntity : class
    {
        private bool _isDisposed = false;

        /// <summary>
        /// The context for this repository.
        /// </summary>
        protected IEfContext Context { get; private set; }

        public EfRepository(IEfContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Context = context;
        }

        #region IEFRepository<TEntity> Support

        /// <summary>
        /// Returns the entity set in the repository.
        /// </summary>
        public IDbSet<TEntity> GetEntities()
        {
            return Context.Get<TEntity>();
        }

        /// <summary>
        /// Returns the entity set in the repository asyncronously.
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetEntitiesAsync()
        {
            return await GetEntities().ToListAsync();
        }
        
        /// <summary>
        /// Returns the entity set in the repository asyncronously after applying the predicate "Where" filter.
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetEntitiesWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetEntities().Where(predicate).ToListAsync();
        }
        
        /// <summary>
        /// Inserts a new TEntity object into the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Create(TEntity entity)
        {
            Context.Create(entity);
        }

        /// <summary>
        /// Updates the existing TEntity object in the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        /// <summary>
        /// Deletes the existing TEntity object from the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Delete(TEntity entity)
        {
            if (Context.GetEntityState(entity) == EntityState.Detached)
                GetEntities().Attach(entity);

            Context.Delete(entity);
        }

        /// <summary>
        /// Saves and finalizes any changes to this repository. 
        /// </summary>
        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// Saves and finalizes any changes to this repository asynchronously. 
        /// </summary>
        public virtual async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Reverts any changes that have been made to this repository.
        /// </summary>
        public virtual void RevertChanges()
        {
            Context.RevertChanges();
        }

        /// <summary>
        /// Reverts any changes that have been made to this repository asynchronously.
        /// </summary>
        public async virtual Task RevertChangesAsync()
        {
            await Context.RevertChangesAsync();
        }

        /// <summary>
        /// Disposes this object and properly cleans up resources. 
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                    Context = null;
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
