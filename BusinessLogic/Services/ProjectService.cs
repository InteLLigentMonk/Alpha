using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class ProjectService(IProjectsRepository projectsRepository)
{
    private readonly IProjectsRepository _projectsRepository = projectsRepository;

    public async Task AddProject(ProjectEntity project)
    {
        await _projectsRepository.BeginTransactionAsync();
        try
        {
            await _projectsRepository.CreateAsync(project);
            await _projectsRepository.SaveAsync();
            await _projectsRepository.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await _projectsRepository.RollbackTransactionAsync();
            throw new Exception("Could not add project", ex);
        }
    }

    public async Task<IEnumerable<ProjectEntity>> GetProjects()
    {
        try
        {
            var result = await _projectsRepository.GetAllAsync();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Could not retrieve projects", ex);
        }
    }

    public async Task<ProjectEntity?> GetProjectById(Guid id)
    {
        try
        {
            var result = await _projectsRepository.GetAsync(x => x.Id == id);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Could not retrieve project", ex);
        }
    }

    public async Task UpdateProject(ProjectEntity project)
    {
        await _projectsRepository.BeginTransactionAsync();
        try
        {
            _projectsRepository.Update(project);
            await _projectsRepository.SaveAsync();
            await _projectsRepository.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await _projectsRepository.RollbackTransactionAsync();
            throw new Exception("Could not update project", ex);
        }
    }

    public async Task DeleteProject(Guid id)
    {
        await _projectsRepository.BeginTransactionAsync();
        try
        {
            var project = await _projectsRepository.GetAsync(x => x.Id == id) ?? throw new Exception("Project not found");
            _projectsRepository.Delete(project);
            await _projectsRepository.SaveAsync();
            await _projectsRepository.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await _projectsRepository.RollbackTransactionAsync();
            throw new Exception("Could not delete project", ex);
        }
    }
}
