using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp.Controllers;

public class AuthController(IWebHostEnvironment env, UserService userService, ProfileService profileService) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly UserService _userService = userService;
    private readonly ProfileService _profileService = profileService;


    public IActionResult Login(string returnUrl="~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;

        var formData = new LoginFormViewModel();
        return View(formData);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginFormViewModel formData, string returnUrl="~/")
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Login";
            return View(formData);
        }

        var result = await _userService.LoginAsync(formData.Email, formData.Password, formData.RememberMe);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        ViewBag.ErrorMessage = "Something went wrong with the login attempt, please try again.";
        return View(formData);


    }

    public IActionResult Register()
    {
        ViewData["Title"] = "Create Account";
        var formData = new RegistrationFormViewModel();
        return View(formData);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationFormViewModel formData)
    {
        if (ModelState.IsValid)
        {
            var (result, _) = await _userService.RegisterAsync(formData, formData.Password);
            if (result.Succeeded)
            {
                ViewData["Title"] = "Dashboard";
                return RedirectToAction("Login", "Auth");
            }
        }
        ViewData["Title"] = "Create Account";
        return View(formData);
    }

    public IActionResult ForgotPassword()
    {
        ViewData["Title"] = "Forgot Password";
        return View();
    }

    [HttpPost]
    public IActionResult ForgotPassword(ForgotPasswordFormViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Forgot Password";
            return View(formData);
        }

        ViewData["Title"] = "Dashboard";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms & Conditions";
        return View();
    }

    public async Task<IActionResult> LogoutAsync()
    {
        await _userService.LogoutAsync();
        return RedirectToAction("Login", "Auth");
    }


    [HttpPost]
    public async Task<IActionResult> AdminAddMember(NewMemberFormViewModel formData, string password = "BytMig123!")
    {
        if (ModelState.IsValid)
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
