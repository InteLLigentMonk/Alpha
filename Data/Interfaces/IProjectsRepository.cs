using Data.Entities;
using Data.Models;
using Domain.Models;
namespace Data.Interfaces;

public interface IProjectsRepository : IBaseRepository<ProjectEntity, Project>
{
    Task<RepositoryResult<bool>> UpdateWithMembersAsync(ProjectEntity updatedEntity);
}
