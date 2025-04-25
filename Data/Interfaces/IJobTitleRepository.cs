using Data.Entities;
using Data.Models;
using Domain.Models;
namespace Data.Interfaces;

public interface IJobTitleRepository : IBaseRepository<JobTitleEntity, JobTitle>
{
    Task<RepositoryResult<bool>> DeleteByIdAsync(int id);
}