using System.Linq.Expressions;
using Data.Models;

namespace Data.Interfaces
{
    public interface IBaseRepository<TEntity, TModel> where TEntity : class where TModel : class
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();


        Task<RepositoryResult<bool>> CreateAsync(TEntity entity);
        Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? orderBy = null, Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);
        Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);
        Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector, bool orderByDescending = false, Expression<Func<TEntity, object>>? orderBy = null, Expression<Func<TEntity, bool>>? filter = null, params Expression<Func<TEntity, object>>[] includes);
        RepositoryResult<bool> Update(TEntity entity);
        RepositoryResult<bool> Delete(TEntity entity);
        Task<RepositoryResult<bool>> DeleteByIdAsync(Guid id);
        Task<RepositoryResult<bool>> Exsists(Expression<Func<TEntity, bool>> expression);
        Task<int> SaveAsync();
    }
}