using BusinessLogic.Factories;
using BusinessLogic.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class UserService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ProfileService profileService)
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly ProfileService _profileService = profileService;


    public async Task<(IdentityResult result, Guid? userId)> RegisterAsync(UserRegistrationForm user, string password)
    {
        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.Email) && await Exists(user.Email))
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Email already exists" }), null);
            }

            AppUser appUser = new()
            {
                Email = user.Email,
                UserName = user.Email
            };
            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
            {
                var profile = ProfileFactory.NewProfileEntity();
                profile.UserId = appUser.Id;
                await _profileService.CreateProfile(profile);

                return (IdentityResult.Success, appUser.Id);
            }
        }
        return (IdentityResult.Failed(new IdentityError { Description = "User cannot be null" }), null);
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
    {
        return await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public IEnumerable<AppUser?> GetAllUsers()
    {
        var users = _userManager.Users
            .Include(u => u.Profile).ToList();

        return users;
    }

    public IEnumerable<Member> GetAllMembers()
    {
        var users = GetAllUsers();
        if (users != null)
        {
            var members = users.Select(u => new Member
            {
                Id = u!.Id,
                FirstName = u.Profile?.FirstName,
                LastName = u.Profile?.LastName,
                PhoneNumber = u.Profile?.PhoneNumber,
                EmailAddress = u.Email,
                JobTitle = u.Profile?.JobTitle,
                AvatarUrl = u.Profile?.AvatarUrl
            });
            return members;
        }
        return [];

    }

    public async Task<bool> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(id));
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        return false;
    }


    public async Task<bool> Exists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}
