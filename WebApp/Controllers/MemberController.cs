using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class MemberController(IWebHostEnvironment env, IUserService userService, IProfileService profileService, IJobTitleService jobTitleService) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IUserService _userService = userService;
    private readonly IProfileService _profileService = profileService;
    private readonly IJobTitleService _jobTitleService = jobTitleService;


    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Members()
    {
        var jobTitleResponse = await _jobTitleService.GetAllJobTitles();
        if (jobTitleResponse.Result != null)
        {
            ViewData["JobTitles"] = jobTitleResponse.Result;
        }

        var vm = new MembersViewModel();
        var memberResponse = await _userService.GetAllMembersAsync();
        if (memberResponse.Result != null)
        {
            vm.Members = memberResponse.Result;
        }

        return View(vm);
    }

    public async Task<IActionResult> CreateAsync()
    {
        var jobTitleResponse = await _jobTitleService.GetAllJobTitles();
        if (jobTitleResponse.Result != null)
        {
            ViewData["JobTitles"] = jobTitleResponse.Result;
        }

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("Partials/_AddMemberFormPartial", new ProfileFormViewModel());
        }

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProfileFormViewModel formData)
    {
        if (ModelState.IsValid)
        {
            var user = new UserRegistrationForm
            {
                Email = formData.Email
            };

            var (result, userId) = await _userService.RegisterAsync(user, "BytMig123!");
            if (result.Succeeded && userId.HasValue)
            {

                Profile model = formData;
                model.UserId = userId;
                model.Id = Guid.NewGuid();

                var jobTitleResponse = await _jobTitleService.GetJobTitle(formData.JobTitle);
                if (jobTitleResponse.Succeeded && jobTitleResponse.Result != null)
                {
                    model.JobTitle = jobTitleResponse.Result;
                }

                if (formData.Avatar != null)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    var uploadResponse = await FileService.UploadImageAsync(formData.Avatar, uploadsFolder);
                    if (uploadResponse.Result != null)
                        model.AvatarUrl = uploadResponse.Result.ToString();
                }

               var response = await _profileService.CreateProfile(model);
                if (response.Succeeded)
                {
                    if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                    {
                        return Json(new { success = true });
                    }
                    return RedirectToAction("Members");
                }

            }

            ModelState.AddModelError("Email", "Email already exists");
            return PartialView("Partials/_AddMemberFormPartial", formData);


        }
        ViewBag.Message = "Something went wrong, please try again later!";
        return PartialView("Partials/_AddMemberFormPartial", formData);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var jobTitleResponse = await _jobTitleService.GetAllJobTitles();
        if (jobTitleResponse.Result != null)
        {
            ViewData["JobTitles"] = jobTitleResponse.Result;
        }

        var response = await _userService.GetMemberById(id);
        if (response.Succeeded) { 
            var vm = new ProfileFormViewModel
            {
                Id = response.Result!.Id,
                UserId = response.Result.UserId,
                FirstName = response.Result.FirstName,
                LastName = response.Result.LastName,
                StreetAddress = response.Result.StreetAddress,
                StreetNumber = response.Result.StreetNumber,
                ZipCode = response.Result.ZipCode,
                City = response.Result.City,
                Country = response.Result.Country,
                Day = response.Result.DateOfBirth!.Value.Day,
                Month = response.Result.DateOfBirth.Value.Month,
                Year = response.Result.DateOfBirth.Value.Year,
                JobTitle = response.Result.JobTitle!.Id,
                Email = response.Result.EmailAddress!,
                PhoneNumber = response.Result.PhoneNumber,
                AvatarUrl = response.Result.AvatarUrl
            };
            return PartialView("Partials/_EditMemberFormPartial", vm);
        }
        ViewBag.Message = "Something went wrong, please try again later!";
        return RedirectToAction("Members");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id,ProfileFormViewModel vm)
    {
        if (ModelState.IsValid)
        {
            Profile model = vm;
            if (vm.Avatar != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var uploadResponse = await FileService.UploadImageAsync(vm.Avatar, uploadsFolder);
                if (uploadResponse.Result != null)
                    model.AvatarUrl = uploadResponse.Result.ToString();
            }
            var profileResponse = await _profileService.UpdateProfile(model);
            if (profileResponse.Succeeded)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("Members");
            }
        }
        ViewBag.Message = "Something went wrong, please try again later!";
        return PartialView("Partials/_EditMemberFormPartial", vm);

    }



    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return RedirectToAction("Members");
        }
        await _userService.Delete(id);
        return RedirectToAction("Members");
    }

}
