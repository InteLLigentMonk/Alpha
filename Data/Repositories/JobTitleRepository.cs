using System.Diagnostics;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;

namespace Data.Repositories;

public class JobTitleRepository(AppDbContext context) : BaseRepository<JobTitleEntity, JobTitle>(context), IJobTitleRepository
{

    public async Task<RepositoryResult<bool>> DeleteByIdAsync(int id)
    {
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
}
