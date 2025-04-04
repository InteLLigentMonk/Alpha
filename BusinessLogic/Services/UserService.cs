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

    public IEnumerable<AppUser> GetAllUsers()
    {
        var users = _userManager.Users
            .Include(u => u.Profile).ToList();
        if (users == null)
        {
            Console.WriteLine("GetAllUsers returned null");
            return [];
        }
        return users;
    }

    public IEnumerable<Member> GetAllMembers()
    {
        try
        {
            var users = GetAllUsers();

            if (users == null)
            {
                Console.WriteLine("GetAllUsers returned null");
                return Enumerable.Empty<Member>();
            }

            var usersList = users.ToList();

            if (usersList.Count == 0)
            {
                Console.WriteLine("No users found");
                return Enumerable.Empty<Member>();
            }

            var members = usersList.Select(u =>
            {
                if (u == null)
                {
                    Console.WriteLine("Encountered a null user in the collection");
                    return null;
                }

                return new Member
                {
                    Id = u.Id,
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    PhoneNumber = u.Profile.PhoneNumber,
                    EmailAddress = u.Email,
                    StreetAddress = u.Profile.StreetAddress,
                    StreetNumber = u.Profile.StreetNumber,
                    ZipCode = u.Profile.ZipCode,
                    City = u.Profile.City,
                    Country = u.Profile.Country,
                    JobTitle = u.Profile.JobTitle,
                    DateOfBirth = u.Profile.DateOfBirth,
                    AvatarUrl = u.Profile.AvatarUrl
                };
            })
            .Where(m => m != null) // Filter out nulls
            .Cast<Member>() // Cast to Member to match the target type
            .ToList();

            return members;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllMembers: {ex.Message}");
            // Log the exception properly if you have a logger
            return Enumerable.Empty<Member>();
        }
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
