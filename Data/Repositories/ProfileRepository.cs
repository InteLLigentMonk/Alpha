using System.Linq.Expressions;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ProfileRepository(AppDbContext context) : BaseRepository<ProfileEntity, Profile>(context), IProfileRepository
{
    public override async Task<RepositoryResult<Profile>> GetAsync(Expression<Func<ProfileEntity, bool>> filter, params Expression<Func<ProfileEntity, object>>[] includes)
    {
        try
        {
            IQueryable<ProfileEntity> query = _dbSet;

            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);

            var entity = await query.FirstOrDefaultAsync(filter);
            if (entity == null)
            {
                return new RepositoryResult<Profile>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Entity not found"
                };
            }

            var result = new Profile
            {
                Id = entity.Id,
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                StreetAddress = entity.StreetAddress,
                StreetNumber = entity.StreetNumber,
                ZipCode = entity.ZipCode,
                City = entity.City,
                Country = entity.Country,
                DateOfBirth = entity.DateOfBirth,
                AvatarUrl = entity.AvatarUrl,
                JobTitle = entity.JobTitle != null ? new JobTitle
                {
                    Id = entity.JobTitle.Id,
                    Title = entity.JobTitle.Title,
                } : null
            };

            return new RepositoryResult<Profile>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = result
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<Profile>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }
}
