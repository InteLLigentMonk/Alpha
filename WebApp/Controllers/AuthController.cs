using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IWebHostEnvironment env, IUserService userService, IProfileService profileService, IAuthService authService) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IUserService _userService = userService;
    private readonly IProfileService _profileService = profileService;
    private readonly IAuthService _authService = authService;


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
            return View(formData);
        }

        var result = await _authService.LoginAsync(formData.Email, formData.Password, formData.RememberMe);
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
            var (result, userId) = await _userService.RegisterAsync(formData, formData.Password);
            if (result.Succeeded)
            {
                var profile = ProfileFactory.NewProfile();
                profile.UserId = userId;
                var response = await _profileService.CreateProfile(profile);
                if(response.Succeeded)
                {
                    ViewData["Title"] = "Dashboard";
                    return RedirectToAction("Login", "Auth");
                }
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
        await _authService.LogoutAsync();
        return RedirectToAction("Login", "Auth");
    }

}
