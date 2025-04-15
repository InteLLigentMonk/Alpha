using BusinessLogic.Models;
using Data.Entities;
using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface IProfileService
    {
        Task<ServiceResult<bool>> CreateProfile(Profile profile);
        Task<ServiceResult<Profile>> GetProfile(Guid userId);
        Task<ServiceResult<bool>> UpdateProfile(Profile profile);
    }
}