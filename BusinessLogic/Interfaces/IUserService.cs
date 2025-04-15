using BusinessLogic.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<bool> Delete(Guid id);
        Task<bool> Exists(string email);
        Task<ServiceResult<IEnumerable<Member>>> GetAllMembersAsync();
        Task<ServiceResult<IEnumerable<AppUser>>> GetAllUsersAsync();
        Task<ServiceResult<Member>> GetMemberById(Guid id);
        Task<ServiceResult<AppUser>> GetUserById(Guid id);
        Task<(IdentityResult result, Guid? userId)> RegisterAsync(UserRegistrationForm user, string password);
    }
}