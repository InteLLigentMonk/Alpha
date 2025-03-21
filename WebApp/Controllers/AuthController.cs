using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        ViewData["Title"] = "Login";

        var formData = new LoginFormModel();

        return View(formData);
    }

    public IActionResult Register()
    {
        ViewData["Title"] = "Create Account";

        var formData = new RegistrationFormModel();

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
    public IActionResult Login(LoginFormModel formData)
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
    public IActionResult Register(RegistrationFormModel formData)
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
    public IActionResult ForgotPassword(ForgotPasswordFormModel formData)
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
