using System.Security.Claims;
using BusinessLogic.Factories;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class ProjectController(IWebHostEnvironment env, IUserService userService, IProfileService profileService, IProjectService projectService, UserManager<AppUser> userManager) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IUserService _userService = userService;
    private readonly IProfileService _profileService = profileService;
    private readonly IProjectService _projectService = projectService;
    private readonly UserManager<AppUser> _userManager = userManager;


    public async Task<IActionResult> All()
    {
        var userResponse = await _userService.GetAllMembersAsync();
        var projectsResponse = await _projectService.GetProjects();
        var vm = new ProjectsViewModel();
        if (userResponse.Result != null && projectsResponse.Result != null)
        {
            vm.Members = userResponse.Result;
            vm.Projects = projectsResponse.Result;
        }
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("Partials/Projects/_CardLoopPartial", vm);
        }
        return View(vm);
    }

    public async Task<IActionResult> Started()
    {
        var userResponse = await _userService.GetAllMembersAsync();
        var projectsResponse = await _projectService.GetStartedProjects();
        var vm = new ProjectsViewModel();
        if (userResponse.Result != null && projectsResponse.Result != null)
        {
            vm.Members = userResponse.Result;
            vm.Projects = projectsResponse.Result;
        }
   
        return PartialView("Partials/Projects/_CardLoopPartial", vm);

    }

    public async Task<IActionResult> Completed()
    {
        var userResponse = await _userService.GetAllMembersAsync();
        var projectsResponse = await _projectService.GetCompletedProjects();
        var vm = new ProjectsViewModel();
        if (userResponse.Result != null && projectsResponse.Result != null)
        {
            vm.Members = userResponse.Result;
            vm.Projects = projectsResponse.Result;
        }
        return PartialView("Partials/Projects/_CardLoopPartial", vm);
    }

    public async Task<IActionResult> Create()
    {

        ViewData["Members"] = (await _userService.GetAllMembersAsync()).Result;

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("Partials/_AddProjectFormPartial", new ProjectFormViewModel());
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormViewModel project)
    {
        if (ModelState.IsValid)
        {
            ViewData["Members"] = await _userService.GetAllMembersAsync();
            Project model = project;

            if (project.ProjectPhoto != null || project.ProjectPhoto?.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var fileName = (await FileService.UploadImageAsync(project.ProjectPhoto, uploadsFolder)).Result;
                model.ProjectPhotoUrl = fileName;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            await _projectService.AddProject(model, userId);

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Projects");

        }

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("Partials/_AddProjectFormPartial", project);
        }

        return View(new ProjectsViewModel { ProjectForm = project });

    }

    public async Task<IActionResult> Edit(Guid id)
    {
        ViewData["Members"] = (await _userService.GetAllMembersAsync()).Result;
        var response = await _projectService.GetProjectById(id);
        if (response.Succeeded && response.Result != null)
        {
            var project = response.Result;
            var formViewModel = new ProjectFormViewModel
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ClientName = project.ClientName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Finished = project.Finished,
                Budget = project.Budget,
                ProjectPhotoUrl = project.ProjectPhotoUrl
            };
            if (project.Users != null)
            {
                formViewModel.Members = [.. project.Users.Select(u => new Member
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    EmailAddress = u.EmailAddress,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    AvatarUrl = u.AvatarUrl
                })];
            }

            return PartialView("Partials/_EditProjectFormPartial", formViewModel);
        }

        ViewBag.Message = "Project not found!";
        return RedirectToAction("Projects");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProjectFormViewModel project)
    {
        if (ModelState.IsValid)
        {
            ViewData["Members"] = await _userService.GetAllMembersAsync();
            Project model = project;

            if (project.ProjectPhoto != null || project.ProjectPhoto?.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var fileName = (await FileService.UploadImageAsync(project.ProjectPhoto, uploadsFolder)).Result;
                model.ProjectPhotoUrl = fileName;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var response = await _projectService.UpdateProject(model, userId);

            if (response.Succeeded)
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Projects");

        }

        return PartialView("_EditProjectFormPartial", project);
    }

    public async Task<IActionResult> AddMembers(Guid id)
    {
        ViewData["Members"] = (await _userService.GetAllMembersAsync()).Result;
        var response = await _projectService.GetProjectById(id);
        if (response.Succeeded && response.Result != null)
        {
            var project = response.Result;
            var formViewModel = new ProjectFormViewModel
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ClientName = project.ClientName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Finished = project.Finished,
                Budget = project.Budget,
                ProjectPhotoUrl = project.ProjectPhotoUrl
            };
            if (project.Users != null)
            {
                formViewModel.Members = [.. project.Users.Select(u => new Member
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    EmailAddress = u.EmailAddress,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    AvatarUrl = u.AvatarUrl
                })];
            }

            return PartialView("Partials/_AddMemberToProjectForm", formViewModel);
        }

        ViewBag.Message = "Project not found!";
        return RedirectToAction("Projects");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMembers(ProjectFormViewModel form)
    {
        if (ModelState.IsValid)
        {
            if(form.Id != null)
            {
                var response = await _projectService.GetProjectById(form.Id.Value);
                if (response.Succeeded && response.Result != null)
                {
                    var project = response.Result;
                    project.Users?.Clear();
                    
                    foreach (var member in form.Members!)
                    {
                        project.Users!.Add(member);
                    }
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
                    var updateResult = await _projectService.UpdateProject(project, userId);

                    if (updateResult.Succeeded)
                    {
                        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                        {
                            return Json(new { success = true });
                        }
                        return RedirectToAction("Projects");
                    }
                }
            }
        }

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            ViewData["Members"] = (await _userService.GetAllMembersAsync()).Result;
            return PartialView("Partials/_AddMemberToProjectForm", form);
        }

        return RedirectToAction("Projects");
    }


    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _projectService.DeleteProject(id);
        if (response.Succeeded)
        {
            ViewBag.Message = "Project deleted successfully!";
            return RedirectToAction("Projects");
        }
        else
        {
            ViewBag.Message = "Error deleting project!";
            return RedirectToAction("Projects");
        }
    }

    public async Task<IActionResult> Finish(Guid id)
    {
        var response = await _projectService.GetProjectById(id);
        if (response.Succeeded && response.Result != null)
        {
            var project = response.Result;
            project.Finished = !project.Finished;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var updateResponse = await _projectService.UpdateProject(project, userId);
            if (updateResponse.Succeeded)
            {
                if(project.Finished)
                {
                    ViewBag.Message = "Project marked as finished!";
                }
                else
                {
                    ViewBag.Message = "Project marked as unfinished!";
                }
                    return RedirectToAction("All");
            }
        }
        ViewBag.Message = "Error finishing project!";
        return RedirectToAction("Projects");
    }
}
