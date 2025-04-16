using System.Diagnostics;
using System.Linq.Expressions;
using Data.Contexts;
using Data.Interfaces;
using Data.Models;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repositories
{
    public abstract class BaseRepository<TEntity, TModel>(AppDbContext context) : IBaseRepository<TEntity, TModel> where TEntity : class where TModel : class
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


        public virtual async Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? orderBy = null, Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (filter != null)
                    query = query.Where(filter);

                if (includes != null && includes.Length != 0)
                    foreach (var include in includes)
                        query = query.Include(include);

                if (orderBy != null)
                    query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

                var entities = await query.ToListAsync();
                var result = entities.Select(e => e.MapTo<TModel>()).ToList();
                return new RepositoryResult<IEnumerable<TModel>>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResult<IEnumerable<TModel>>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }

        public virtual async Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector,bool orderByDescending = false, Expression<Func<TEntity, object>>? orderBy = null, Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (filter != null)
                    query = query.Where(filter);

                if (includes != null && includes.Length != 0)
                    foreach (var include in includes)
                        query = query.Include(include);

                if (orderBy != null)
                    query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

                var entities = await query.Select(selector).ToListAsync();
                return new RepositoryResult<IEnumerable<TSelect>>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = entities
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResult<IEnumerable<TSelect>>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }


        public virtual async Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;

                if (includes != null && includes.Length != 0)
                    foreach (var include in includes)
                        query = query.Include(include);

                var entity = await query.FirstOrDefaultAsync(filter);
                if(entity == null)
                {
                    return new RepositoryResult<TModel>
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Error = "Entity not found"
                    };
                }

                var result = entity.MapTo<TModel>();
                return new RepositoryResult<TModel>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResult<TModel>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }


        public virtual async Task<RepositoryResult<bool>> CreateAsync(TEntity entity)
        {
            if (entity == null)
            {
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "Entity can't be null"
                };
            }

            try
            {
                await _dbSet.AddAsync(entity);
                return new RepositoryResult<bool>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }

        public virtual RepositoryResult<bool> Update(TEntity entity)
        {
            if (entity == null)
            {
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "Entity can't be null"
                };
            }

            try
            {
                _dbSet.Update(entity);
                return new RepositoryResult<bool>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exeption Message: {ex.Message}");
                Debug.WriteLine($"Inner Exeption Message: {ex.InnerException?.Message}");
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }

        public virtual RepositoryResult<bool> Delete(TEntity entity)
        {
            if(entity == null)
            {
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "Entity can't be null"
                };
            }

            try
            {
                _dbSet.Remove(entity);
                return new RepositoryResult<bool>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }

        public virtual async Task<RepositoryResult<bool>> DeleteByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "Id can't be Empty"
                };
            }

            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    return new RepositoryResult<bool>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = true
                    };
                }
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Entity not found"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = ex.Message
                };
            }
        }

        public virtual async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveAsync failed: {ex.Message} {ex.InnerException}");
                throw;
            }
        }

        public virtual async Task<RepositoryResult<bool>> Exsists(Expression<Func<TEntity, bool>> expression)
        {
            var exists = await _dbSet.AnyAsync(expression);
            return exists
                ? new RepositoryResult<bool>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = true
                }
                : new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Entity not found"
                };

        }

        #endregion
    }
}
