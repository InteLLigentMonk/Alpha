using System.Linq.Expressions;
using Data.Entities;
using Data.Models;
using Domain.Models;
namespace Data.Interfaces;

public interface IProjectsRepository : IBaseRepository<ProjectEntity, Project>
{
    Task<RepositoryResult<Project>> GetProjectWithProfileAsync(Expression<Func<ProjectEntity, bool>> filter);
    Task<RepositoryResult<bool>> UpdateWithMembersAsync(ProjectEntity updatedEntity);
}
