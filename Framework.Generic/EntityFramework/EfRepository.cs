using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Framework.Generic.EntityFramework
{
    public interface IEfRepository<TEntity> : IDisposable where TEntity : class
    {
        IDbSet<TEntity> GetEntities();
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void SaveChanges();
        void RevertChanges();
    }

    public class EfRepository<TEntity> : IEfRepository<TEntity> where TEntity : class
    {
        private bool _isDisposed = false;
        private readonly IEfContext _context;

        public EfRepository(IEfContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        #region IEFRepository<TEntity> Support

        /// <summary>
        /// Returns the entity set in the repository.
        /// </summary>
        public IDbSet<TEntity> GetEntities()
        {
            return _context.Get<TEntity>();
        }
        
        /// <summary>
        /// Inserts a new TEntity object into the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Add(TEntity entity)
        {
            _context.Add(entity);
        }

        /// <summary>
        /// Inserts new TEntity objects into the entity set. Note: Changes are not saved.
        /// </summary>
        /// <param name="entities"></param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.AddRange(entities);
        }

        /// <summary>
        /// Updates the existing TEntity object in the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Update(TEntity entity)
        {
            _context.Update(entity);
        }

        /// <summary>
        /// Deletes the existing TEntity object from the entity set. Note: Changes are not saved.
        /// </summary>
        public virtual void Delete(TEntity entity)
        {
            if (_context.GetEntityState(entity) == EntityState.Detached)
                GetEntities().Attach(entity);

            _context.Delete(entity);
        }

        /// <summary>
        /// Saves and finalizes any changes to this repository. 
        /// </summary>
        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }


        /// <summary>
        /// Reverts any changes that have been made to this repository.
        /// </summary>
        public virtual void RevertChanges()
        {
            _context.RevertChanges();
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
                    _context.Dispose();
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
