using System.Diagnostics;
using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Data.Interfaces;
using Domain.Models;

namespace BusinessLogic.Services;

public class ProjectService(IProjectsRepository projectsRepository, ProjectFactory projectFactory) : IProjectService
{
    private readonly IProjectsRepository _projectsRepository = projectsRepository;
    private readonly ProjectFactory _projectFactory = projectFactory;

    public async Task<ServiceResult<bool>> AddProject(Project project)
    {
        if (project != null)
        {
            var projectEntity = await _projectFactory.Project(project);

            await _projectsRepository.BeginTransactionAsync();
            try
            {
                await _projectsRepository.CreateAsync(projectEntity);
                await _projectsRepository.SaveAsync();
                await _projectsRepository.CommitTransactionAsync();
                return new ServiceResult<bool>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                await _projectsRepository.RollbackTransactionAsync();
                Debug.WriteLine(ex.Message);
                return new ServiceResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = "Could not create project",
                    Result = false
                };
            }
        }
        return new ServiceResult<bool>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Project can't be null",
            Result = false
        };

    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjects()
    {
        try
        {
            var result = await _projectsRepository.GetAllAsync(p => new Project
            {
                Id = p.Id,
                ProjectName = p.ProjectName,
                ClientName = p.ClientName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Finished = p.Finished,
                Budget = p.Budget,
                ProjectPhotoUrl = p.ProjectPhotoUrl,
                Users = p.Users.Select(u => new Member
                {
                    Id = u.Profile.Id,
                    EmailAddress = u.Email,
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    AvatarUrl = u.Profile.AvatarUrl,
                    UserId = u.Profile.UserId
                }).ToList()
            },
            includes: [p => p.Users]
            );
            if (result == null)
            {
                return new ServiceResult<IEnumerable<Project>>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "No projects found",
                };
            }
            ;

            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = result.Result
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = "Could not retrieve projects"
            };
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetStartedProjects()
    {
        try
        {
            var result = await _projectsRepository.GetAllAsync(p => new Project
            {
                Id = p.Id,
                ProjectName = p.ProjectName,
                ClientName = p.ClientName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Finished = p.Finished,
                Budget = p.Budget,
                ProjectPhotoUrl = p.ProjectPhotoUrl,
                Users = p.Users.Select(u => new Member
                {
                    Id = u.Profile.Id,
                    EmailAddress = u.Email,
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    AvatarUrl = u.Profile.AvatarUrl,
                    UserId = u.Profile.UserId
                }).ToList()
            },
            filter: x => x.StartDate.Date < DateTime.Today.Date && x.Finished == false,
            includes: [p => p.Users]
            );
            if (result == null)
            {
                return new ServiceResult<IEnumerable<Project>>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "No projects found",
                };
            }
            ;

            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = result.Result
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = "Could not retrieve projects"
            };
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetCompletedProjects()
    {
        try
        {
            var result = await _projectsRepository.GetAllAsync(p => new Project
            {
                Id = p.Id,
                ProjectName = p.ProjectName,
                ClientName = p.ClientName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Finished = p.Finished,
                Budget = p.Budget,
                ProjectPhotoUrl = p.ProjectPhotoUrl,
                Users = p.Users.Select(u => new Member
                {
                    Id = u.Profile.Id,
                    EmailAddress = u.Email,
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    AvatarUrl = u.Profile.AvatarUrl,
                    UserId = u.Profile.UserId
                }).ToList()
            },
            filter: x => x.Finished == true,
            includes: [p => p.Users]
            );
            if (result == null)
            {
                return new ServiceResult<IEnumerable<Project>>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "No projects found",
                };
            }
            ;

            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = result.Result
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = "Could not retrieve projects"
            };
        }
    }

    public async Task<ServiceResult<Project>> GetProjectById(Guid id)
    {
        if (id != Guid.Empty)
        {
            try
            {
                var model = await _projectsRepository.GetProjectWithProfileAsync(x => x.Id == id);
                if (model == null)
                {
                    return new ServiceResult<Project>
                    {
                        Succeeded = false,
                        StatusCode = 404,
                        Error = "Project not found",
                    };
                }
                return new ServiceResult<Project>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = model.Result
                };

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new ServiceResult<Project>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = "Could not retrieve project"
                };
            }
        }
        return new ServiceResult<Project>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Id can't be empty",
        };
    }

    public async Task<ServiceResult<bool>> UpdateProject(Project project)
    {
        if (project != null)
        {
            await _projectsRepository.BeginTransactionAsync();
            try
            {
                var projectEntity = await _projectFactory.Project(project);

                var result = await _projectsRepository.UpdateWithMembersAsync(projectEntity);
                if (result.Succeeded)
                {
                    await _projectsRepository.SaveAsync();
                    await _projectsRepository.CommitTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = true
                    };
                }
                else
                {
                    await _projectsRepository.RollbackTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = false,
                        StatusCode = result.StatusCode,
                        Error = result.Error,
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                await _projectsRepository.RollbackTransactionAsync();
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.InnerException?.Message);
                return new ServiceResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = "Could not update project",
                    Result = false
                };
            }
        }
        return new ServiceResult<bool>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Project can't be null",
            Result = false
        };
    }

    public async Task<ServiceResult<bool>> DeleteProject(Guid id)
    {
        if (id != Guid.Empty)
        {
            await _projectsRepository.BeginTransactionAsync();
            try
            {
                    await _projectsRepository.DeleteByIdAsync(id);
                    await _projectsRepository.SaveAsync();
                    await _projectsRepository.CommitTransactionAsync();
                    return new ServiceResult<bool>
                    {
                        Succeeded = true,
                        StatusCode = 200,
                        Result = true
                    };
            }
            catch (Exception ex)
            {
                await _projectsRepository.RollbackTransactionAsync();
                Debug.WriteLine(ex.Message);
                return new ServiceResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = "Could not delete project",
                    Result = false
                };
            }
        }
        return new ServiceResult<bool>
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Id can't be empty",
            Result = false
        };
    }
}
