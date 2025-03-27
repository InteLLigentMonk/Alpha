using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(UserService userService) : Controller
{
    private readonly UserService _userService = userService;


    
    public IActionResult Login()
    {
        ViewData["Title"] = "Login";

        var formData = new LoginFormViewModel();

        return View(formData);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginFormViewModel formData)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.LoginAsync(formData.Email, formData.Password, formData.RememberMe);
            if (result.Succeeded)
            {
                ViewData["Title"] = "Dashboard";
                return RedirectToAction("Projects", "Home");
            }
        }

        ViewData["Title"] = "Login";
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
            var result = await _userService.RegisterAsync(formData, formData.Password);
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
