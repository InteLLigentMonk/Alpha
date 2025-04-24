using System.Security.Claims;
using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IWebHostEnvironment env, IUserService userService, IProfileService profileService, IAuthService authService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IUserService _userService = userService;
    private readonly IProfileService _profileService = profileService;
    private readonly IAuthService _authService = authService;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly UserManager<AppUser> _userManager = userManager;

    #region Local Identity
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

        var result = await _authService.LoginAsync(formData.Email.ToLower(), formData.Password, formData.RememberMe);
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
    #endregion

    #region External Login

    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid provider");
            return View("Login");
        }

        var redirectUrl = Url.Action("ExternalSignInCallback", "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("~/");
        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            return View("Login");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if(info == null)
            return RedirectToAction("Login");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!.ToLower();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var linkResult = await _userManager.AddLoginAsync(existingUser, info);
                if (linkResult.Succeeded)
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in linkResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Login");
            } else
            {

                var user = new AppUser
                {
                    UserName = email,
                    Email = email
                };

                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);

                    var profile = ProfileFactory.NewProfile();
                    profile.UserId = user.Id;
                    var response = await _profileService.CreateProfile(profile);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Login");
            }
        }
    }

    #endregion
}
