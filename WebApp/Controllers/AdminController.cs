using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Services;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AdminController(IWebHostEnvironment env, IUserService userService, IProfileService profileService) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IUserService _userService = userService;
    private readonly IProfileService _profileService = profileService;


    [HttpPost]
    public async Task<IActionResult> AddMember(ProfileFormViewModel formData, string password = "BytMig123!")
    {
        if (ModelState.IsValid)
        {
            if (formData.Id != Guid.Empty)
            {
                Profile model = formData;
                if (formData.Avatar != null)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    var uploadResponse = await FileService.UploadImageAsync(formData.Avatar, uploadsFolder);
                    if(uploadResponse.Result != null)
                        model.AvatarUrl = uploadResponse.Result.ToString();
                }
                await _profileService.UpdateProfile(model);
            }
            else
            {
                var user = new UserRegistrationForm
                {
                    Email = formData.Email
                };

                var (result, userId) = await _userService.RegisterAsync(user, password);
                if (result.Succeeded && userId.HasValue)
                {
                    
                    Profile model = formData;
                    model.UserId = userId;
                    model.Id = Guid.NewGuid();

                    if (formData.Avatar != null)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        var uploadResponse = await FileService.UploadImageAsync(formData.Avatar, uploadsFolder);
                        if (uploadResponse.Result != null)
                            model.AvatarUrl = uploadResponse.Result.ToString();
                    }
                    
                    await _profileService.CreateProfile(model);
                    

                }
            }
        }
        return RedirectToAction("Members", "Home");
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return RedirectToAction("Members", "Home");
        }
        await _userService.Delete(id);
        return RedirectToAction("Members", "Home");
    }

}
