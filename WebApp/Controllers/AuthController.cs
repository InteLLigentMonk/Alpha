using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

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

}
