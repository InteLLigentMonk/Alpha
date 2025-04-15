using System.Diagnostics;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ProjectsRepository(AppDbContext context) : BaseRepository<ProjectEntity, Project>(context), IProjectsRepository
{

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
