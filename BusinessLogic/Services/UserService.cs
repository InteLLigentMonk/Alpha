using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class UserService(UserManager<AppUser> userManager, IProfileService profileService, INotificationService notificationService) : IUserService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IProfileService _profileService = profileService;
    private readonly INotificationService _notificationService = notificationService;


    public async Task<(IdentityResult result, Guid? userId)> RegisterAsync(UserRegistrationForm user, string password, string creatorId)
    {
        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.Email) && await Exists(user.Email))
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Email already exists" }), null);
            }

            AppUser appUser = new()
            {
                Email = user.Email.ToLower(),
                UserName = user.Email.ToLower()
            };
            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
            {
                var creator = await _userManager.FindByIdAsync(creatorId);
                if (creator != null)
                {
                    var response = await GetMemberById(creator.Id);
                    if (response.Succeeded && response.Result != null)
                    {
                        var userWithProfile = response.Result;
                        var notificationEntity = new NotificationEntity
                        {
                            Message = $"A new user was added by {userWithProfile.FirstName} {userWithProfile.LastName}",
                            NotificationTypeId = 1,
                            NotificationTargetGroupId = 2,
                            CreatedByUserId = creator.Id.ToString()
                        };

                        if (string.IsNullOrEmpty(userWithProfile.AvatarUrl))
                        {
                            notificationEntity.Icon = "/icons/avatars/2.svg";
                        }
                        else
                        {
                            notificationEntity.Icon = $"/uploads/{userWithProfile.AvatarUrl}";
                        }

                        await _notificationService.AddNotificationAsync(notificationEntity, creator.Id.ToString());
                    }
                }

                return (IdentityResult.Success, appUser.Id);
            }
        }
        return (IdentityResult.Failed(new IdentityError { Description = "User can't' be null" }), null);
    }

    public async Task<ServiceResult<AppUser>> GetUserById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new ServiceResult<AppUser>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Id can't be empty",
                Result = null
            };
        }
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .ThenInclude(p => p.JobTitle)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user == null
            ? new ServiceResult<AppUser>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "User Not found"
            }
            : new ServiceResult<AppUser>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = user
            };
    }

    public async Task<ServiceResult<Member>> GetMemberById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new ServiceResult<Member>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Id cannot be empty",
                Result = null
            };
        }
        var userResult = await GetUserById(id);
        if (userResult.Result == null)
        {
            return new ServiceResult<Member>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "Member not found"
            };
        }
        return new ServiceResult<Member>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = UserFactory.Member(userResult.Result)
        };
    }


    public async Task<ServiceResult<IEnumerable<AppUser>>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Include(u => u.Profile)
            .ThenInclude(p => p.JobTitle)
            .ToListAsync();
        if (users == null)
        {
            return new ServiceResult<IEnumerable<AppUser>>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "No users found"
            };
        }
        return new ServiceResult<IEnumerable<AppUser>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = users
        };
    }



    public async Task<ServiceResult<IEnumerable<SimpleUser>>> GetSimpleUsersAsync()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var users = allUsers
            .Select(u => new SimpleUser
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!
            }).ToList();
        var userDictionary = allUsers.ToDictionary(u => u.Id);
        foreach (var user in users)
        {
            if (userDictionary.TryGetValue(user.Id, out var appUser))
            {
                user.Roles = await _userManager.GetRolesAsync(appUser);
            }
        }

        if (users == null || users.Count == 0)
        {
            return new ServiceResult<IEnumerable<SimpleUser>>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "No users found"
            };
        }

        return new ServiceResult<IEnumerable<SimpleUser>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = users
        };
    }

    public async Task<ServiceResult<IEnumerable<Member>>> GetAllMembersAsync()
    {
        var users = await GetAllUsersAsync();
        if (users.Result != null)
        {
            var usersList = users.Result.ToList();
            if (usersList.Count != 0)
            {
                IEnumerable<Member> members = usersList.Select(u => UserFactory.Member(u)).ToList();

                return new ServiceResult<IEnumerable<Member>>
                {
                    Succeeded = true,
                    StatusCode = 200,
                    Result = members
                };
            }
        }
        return new ServiceResult<IEnumerable<Member>>
        {
            Succeeded = false,
            StatusCode = 404,
            Error = "No members found"
        };
    }

    public async Task<bool> Delete(Guid id)
    {
        if (id != Guid.Empty)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
        }
        return false;
    }


    public async Task<bool> Exists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}
