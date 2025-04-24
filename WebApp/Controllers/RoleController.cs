using BusinessLogic.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Roles;

namespace WebApp.Controllers;

public class RoleController(RoleManager<AppRole> roleManager, IUserService userService, UserManager<AppUser> userManager) : Controller
{
    private readonly RoleManager<AppRole> _roleManager = roleManager;
    private readonly IUserService _userService = userService;
    private readonly UserManager<AppUser> _userManager = userManager;

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Roles()
    {
        ViewData["Title"] = "Roles";
        var vm = new RolesViewModel();
        var response = await _userService.GetSimpleUsersAsync();
        if (response.Succeeded && response.Result != null)
        {
            vm.Roles = [.. _roleManager.Roles];
            vm.Users = response.Result;
        }
        return View(vm);
    }

    public IActionResult Create()
    {
        return PartialView("Partials/_AddRoleFormPartial", new AddRoleFormViewModel());
    }


    [HttpPost]
    public async Task<IActionResult> Create(AddRoleFormViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var role = new AppRole
            {
                Name = vm.Role,
            };

            var existingRole = await _roleManager.FindByNameAsync(role.Name);
            if (existingRole != null)
            {
                ModelState.AddModelError("Role", "Role already exists");
                return PartialView("Partials/_AddRoleFormPartial", vm);
            }

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction("Roles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("Partials/_AddRoleFormPartial", vm);
        }

        return View("Roles", vm);
    }

    public async Task<IActionResult> AddMemberToRole()
    {
        var response = await _userService.GetSimpleUsersAsync();
        if (response.Succeeded && response.Result != null)
        {
            ViewData["Roles"] = _roleManager.Roles.ToList();
            ViewData["Users"] = response.Result.ToList();
        }
        return PartialView("Partials/_AddMemberToRoleFormPartial", new AssignRoleToUserFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> AddMemberToRole(AssignRoleToUserFormViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user != null)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, vm.RoleName);
                if (isInRole)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"User already has the role '{vm.RoleName}'."
                    });
                }

                var response = await _userManager.AddToRoleAsync(user, vm.RoleName);
                if (response.Succeeded)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Role added successfully."
                    });
                }

                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found");
            }
        }

        var userResponse = await _userService.GetSimpleUsersAsync();
        if (userResponse.Succeeded && userResponse.Result != null)
        {
            ViewData["Roles"] = _roleManager.Roles.ToList();
            ViewData["Users"] = userResponse.Result.ToList();
        }

        return PartialView("Partials/_AddMemberToRoleFormPartial", vm);
    }


    public async Task<IActionResult> DeleteAsync(string id, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is not null)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("Roles");
            }
        }

        ViewBag.ErrorMessage = "User not found";
        return PartialView("Partials/_DeleteRoleFormPartial");
    }
}
