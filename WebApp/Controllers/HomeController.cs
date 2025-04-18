using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class HomeController(IWebHostEnvironment env, IUserService userService, IProfileService profileService, IProjectService projectService, UserManager<AppUser> userManager) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IUserService _userService = userService;
    private readonly IProfileService _profileService = profileService;
    private readonly IProjectService _projectService = projectService;
    private readonly UserManager<AppUser> _userManager = userManager;

    public IActionResult Dashboard()
    {
        return View();
    }


    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }
        var userProfile = await _profileService.GetProfile(user.Id);
        if (userProfile.Result == null)
        {
            return NotFound();
        }
        var profileViewModel = userProfile.Result.MapTo<ProfileFormViewModel>();
        profileViewModel.UserId = user.Id;
        profileViewModel.Email = user.Email!;
        profileViewModel.Day = userProfile.Result.DateOfBirth.Day;
        profileViewModel.Month = userProfile.Result.DateOfBirth.Month;
        profileViewModel.Year = userProfile.Result.DateOfBirth.Year;


        return View(profileViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileFormViewModel profile)
    {
        Profile model = profile;
        if (profile.Avatar != null)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var uploadResponse = await FileService.UploadImageAsync(profile.Avatar, uploadsFolder);
            if (uploadResponse.Result != null)
                model.AvatarUrl = uploadResponse.Result.ToString();
        }

        if (ModelState.IsValid)
        {
            await _profileService.UpdateProfile(model);

            ViewBag.Message = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        ViewBag.Message = "Something went wrong when trying to update profile, please try again later!";
        return View(profile);

    }
}
