using BusinessLogic.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public class AuthService(SignInManager<AppUser> signInManager) : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
    {
        return await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
