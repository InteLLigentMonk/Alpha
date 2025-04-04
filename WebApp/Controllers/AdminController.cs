using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AdminController(IWebHostEnvironment env, UserService userService, ProfileService profileService) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly UserService _userService = userService;
    private readonly ProfileService _profileService = profileService;


    [HttpPost]
    public async Task<IActionResult> AddMember(NewMemberFormViewModel formData, string password = "BytMig123!")
    {
        if (ModelState.IsValid)
        {
            if (formData.Id.HasValue)
            {
                var userProfile = await _profileService.GetProfile(formData.Id.Value);
                if (userProfile != null)
                {
                    userProfile.FirstName = formData.FirstName;
                    userProfile.LastName = formData.LastName;
                    userProfile.PhoneNumber = formData.PhoneNumber;
                    userProfile.JobTitle = formData.JobTitle;
                    userProfile.StreetAddress = formData.StreetAddress;
                    userProfile.StreetNumber = formData.StreetNumber;
                    userProfile.City = formData.City;
                    userProfile.ZipCode = formData.ZipCode;
                    userProfile.Country = formData.Country;
                    if (formData.DateOfBirth.HasValue)
                    {
                        userProfile.DateOfBirth = formData.DateOfBirth.Value;
                    }
                    if (formData.Avatar != null)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(formData.Avatar.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await formData.Avatar.CopyToAsync(fileStream);
                        }
                        userProfile.AvatarUrl = fileName;
                    }
                    await _profileService.UpdateProfile(userProfile);
                }
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
                    var userProfile = await _profileService.GetProfile(userId.Value);
                    if (userProfile != null)
                    {
                        userProfile.FirstName = formData.FirstName;
                        userProfile.LastName = formData.LastName;
                        userProfile.PhoneNumber = formData.PhoneNumber;
                        userProfile.JobTitle = formData.JobTitle;
                        userProfile.StreetAddress = formData.StreetAddress;
                        userProfile.StreetNumber = formData.StreetNumber;
                        userProfile.City = formData.City;
                        userProfile.ZipCode = formData.ZipCode;
                        userProfile.Country = formData.Country;

                        if (formData.DateOfBirth.HasValue)
                        {
                            userProfile.DateOfBirth = formData.DateOfBirth.Value;
                        }

                        if (formData.Avatar != null)
                        {
                            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(formData.Avatar.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await formData.Avatar.CopyToAsync(fileStream);
                            }
                            userProfile.AvatarUrl = fileName;
                        }

                        await _profileService.UpdateProfile(userProfile);
                    }

                }
            }
        }
        return RedirectToAction("Members", "Home");
    }

    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return RedirectToAction("Members", "Home");
        }
        await _userService.Delete(id);
        return RedirectToAction("Members", "Home");
    }

}
