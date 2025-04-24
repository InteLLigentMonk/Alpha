using BusinessLogic.Models;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IProjectService
    {
        Task<ServiceResult<bool>> AddProject(Project project);
        Task<ServiceResult<bool>> DeleteProject(Guid id);
        Task<ServiceResult<IEnumerable<Project>>> GetCompletedProjects();
        Task<ServiceResult<Project>> GetProjectById(Guid id);
        Task<ServiceResult<IEnumerable<Project>>> GetProjects();
        Task<ServiceResult<IEnumerable<Project>>> GetStartedProjects();
        Task<ServiceResult<bool>> UpdateProject(Project project);
    }
}