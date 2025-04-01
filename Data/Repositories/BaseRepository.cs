using System.Linq.Expressions;
using Data.Contexts;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repositories
{
    public abstract class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context = context;
        protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
        private IDbContextTransaction _transaction = null!;

        #region Transaction Management

        public virtual async Task BeginTransactionAsync()
        {
            _transaction ??= await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                _transaction.Dispose();
                _transaction = null!;
            }
        }

        public virtual async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null!;
            }
        }

        #endregion

        #region CRUD Operations

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not retrieve entities", ex);
            }
        }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            try
            {
                return await _dbSet.FirstOrDefaultAsync(expression);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not retrieve entity", ex);
            }
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create entity", ex);
            }
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not update entity", ex);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            try
            {
                _dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not delete entity", ex);
            }
        }

        public virtual async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Task<bool> Exsists(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.AnyAsync(expression);
        }

        #endregion
    }
}
