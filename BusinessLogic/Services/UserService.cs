using BusinessLogic.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public class UserService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    public async Task<IdentityResult> RegisterAsync(UserRegistrationForm user, string password)
    {
        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.Email) && await Exists(user.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists" });
            }
            AppUser appUser = new() { Email = user.Email, UserName = user.Email };
            await _userManager.CreateAsync(appUser, password);
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(new IdentityError { Description = "User cannot be null" });
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
    {
        return await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> Exists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}
