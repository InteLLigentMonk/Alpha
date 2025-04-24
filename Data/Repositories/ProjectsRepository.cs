using System.Diagnostics;
using System.Linq.Expressions;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ProjectsRepository(AppDbContext context) : BaseRepository<ProjectEntity, Project>(context), IProjectsRepository
{

    public async Task<RepositoryResult<Project>> GetProjectWithProfileAsync(Expression<Func<ProjectEntity, bool>> filter)
    {
        try
        {

            var entity = await _dbSet
                .Include(p => p.Users)
                .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(filter);

            if (entity == null)
            {
                return new RepositoryResult<Project>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Entity not found"
                };
            }

            var result = new Project
            {
                Id = entity.Id,
                ProjectName = entity.ProjectName,
                ClientName = entity.ClientName,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Finished = entity.Finished,
                Budget = entity.Budget,
                ProjectPhotoUrl = entity.ProjectPhotoUrl,
                Users = entity.Users.Select(u => new Member
                {
                    Id = u.Profile.Id,
                    EmailAddress = u.Email,
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    AvatarUrl = u.Profile.AvatarUrl,
                    UserId = u.Id
                }).ToList()
            };
            return new RepositoryResult<Project>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = result
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<Project>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }

    public async Task<RepositoryResult<bool>> UpdateWithMembersAsync(ProjectEntity updatedEntity)
    {
        try
        {
            // Get existing entity with its relationships
            var existingEntity = await _dbSet
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == updatedEntity.Id);

            if (existingEntity == null)
            {
                return new RepositoryResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Project not found"
                };
            }

            // Update properties
            existingEntity.ProjectName = updatedEntity.ProjectName;
            existingEntity.ClientName = updatedEntity.ClientName;
            existingEntity.Description = updatedEntity.Description;
            existingEntity.Finished = updatedEntity.Finished;
            existingEntity.StartDate = updatedEntity.StartDate;
            existingEntity.EndDate = updatedEntity.EndDate;
            existingEntity.Budget = updatedEntity.Budget;

            if (!string.IsNullOrEmpty(updatedEntity.ProjectPhotoUrl))
            {
                existingEntity.ProjectPhotoUrl = updatedEntity.ProjectPhotoUrl;
            }

            // Clear and re-add users to avoid duplicate key issues
            existingEntity.Users.Clear();
            foreach (var user in updatedEntity.Users)
            {
                existingEntity.Users.Add(user);
            }

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
            Debug.WriteLine(ex.InnerException?.Message);
            return new RepositoryResult<bool>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }

}
