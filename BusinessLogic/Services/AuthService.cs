using BusinessLogic.Hubs;
using BusinessLogic.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
namespace BusinessLogic.Services;

public class AuthService(SignInManager<AppUser> signInManager, INotificationService notificationService, IUserService userService, UserManager<AppUser> userManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var response = await _userService.GetMemberById(user.Id);
                if (response.Succeeded && response.Result != null)
                {
                    var userWithProfile = response.Result;
                    var notificationEntity = new NotificationEntity
                    {
                        Message = $"{userWithProfile.FirstName} {userWithProfile.LastName} signed in.",
                        NotificationTypeId = 1,
                        NotificationTargetGroupId = 2,
                        Icon = $"/uploads/{userWithProfile.AvatarUrl}",
                        CreatedByUserId = user.Id.ToString()
                    };
                    await _notificationService.AddNotificationAsync(notificationEntity, user.Id.ToString());

                }
            }

        }

        return result;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
