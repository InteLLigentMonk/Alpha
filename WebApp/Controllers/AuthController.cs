using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        ViewData["Title"] = "Login";

        var formData = new LoginFormViewModel();

        return View(formData);
    }

    public IActionResult Register()
    {
        ViewData["Title"] = "Create Account";

        var formData = new RegistrationFormViewModel();

        return View(formData);
    }

    public IActionResult ForgotPassword()
    {
        ViewData["Title"] = "Forgot Password";


        return View();
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms & Conditions";

        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginFormViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Login";
            return View(formData);
        }

        ViewData["Title"] = "Dashboard";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Register(RegistrationFormViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Create Account";
            return View(formData);
        }

        ViewData["Title"] = "Dashboard";
        return RedirectToAction("Index", "Home");
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
}
