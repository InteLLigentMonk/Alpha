using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task CreateAsync(TEntity entity);
        void Delete(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression);
        Task RollbackTransactionAsync();
        Task<int> SaveAsync();
        void Update(TEntity entity);
        Task<bool> Exsists(Expression<Func<TEntity, bool>> expression);
    }
}