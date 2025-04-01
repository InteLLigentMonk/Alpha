using BusinessLogic.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class HomeController(IWebHostEnvironment env, UserService userService, ProfileService profileService, UserManager<AppUser> userManager) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly UserService _userService = userService;
    private readonly ProfileService _profileService = profileService;
    private readonly UserManager<AppUser> _userManager = userManager;

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Projects()
    {
        return View(new ProjectsViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Projects(ProjectFormViewModel project)
    {
        if (!ModelState.IsValid || project.ProjectPhoto == null || project.ProjectPhoto.Length == 0)
            return View(new ProjectsViewModel { ProjectForm = project });

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{Path.GetFileName(project.ProjectPhoto.FileName)}");
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await project.ProjectPhoto.CopyToAsync(fileStream);
        }

        ViewBag.Message = "Project added successfully!";

        return RedirectToAction("Projects");
    }


    public IActionResult Members()
    {
        var vm = new MembersViewModel
        {
            Members = _userService.GetAllMembers()
        };
        return View(vm);
    }


    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }
        var userProfile = await _profileService.GetProfile(user.Id);
        if (userProfile == null)
        {
            return NotFound();
        }
        var profileViewModel = new ProfileFormViewModel
        {
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            PhoneNumber = userProfile.PhoneNumber,
            JobTitle = userProfile.JobTitle,
            StreetAddress = userProfile.StreetAddress,
            StreetNumber = userProfile.StreetNumber,
            ZipCode = userProfile.ZipCode,
            City = userProfile.City,
            Country = userProfile.Country,
            Day = userProfile.DateOfBirth.Day,
            Month = userProfile.DateOfBirth.Month,
            Year = userProfile.DateOfBirth.Year,
            AvatarUrl = userProfile.AvatarUrl
        };

        return View(profileViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileFormViewModel profile)
    {
        if( profile.Avatar != null && profile.Avatar.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(profile.Avatar.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profile.Avatar.CopyToAsync(fileStream);
            }
            profile.AvatarUrl = fileName;
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var userProfile = await _profileService.GetProfile(user.Id);
            if (userProfile == null)
            {
                return NotFound();
            }
            userProfile.FirstName = profile.FirstName;
            userProfile.LastName = profile.LastName;
            userProfile.PhoneNumber = profile.PhoneNumber;
            userProfile.JobTitle = profile.JobTitle;
            userProfile.StreetAddress = profile.StreetAddress;
            userProfile.StreetNumber = profile.StreetNumber;
            userProfile.ZipCode = profile.ZipCode;
            userProfile.City = profile.City;
            userProfile.Country = profile.Country;
            userProfile.DateOfBirth = new DateTime(profile.Year, profile.Month, profile.Day);
            if(!string.IsNullOrEmpty(profile.AvatarUrl))
                userProfile.AvatarUrl = profile.AvatarUrl;

            await _profileService.UpdateProfile(userProfile);

            ViewBag.Message = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        ViewBag.Message = "Something went wrong when trying to update profile, please try again later!";
        return View(profile);

    }
}
