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


    public async Task<IActionResult> Projects()
    {
        var userResponse = await _userService.GetAllMembersAsync();
        var projectsResponse = await _projectService.GetProjects();
        var vm = new ProjectsViewModel();
        if (userResponse.Result != null && projectsResponse.Result != null)
        {
            vm.Members = userResponse.Result;
            vm.Projects = projectsResponse.Result;
        }
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Projects(ProjectFormViewModel project)
    {
        if (!ModelState.IsValid)
            return View(new ProjectsViewModel { ProjectForm = project });

        Project model = project;

        if (project.ProjectPhoto != null || project.ProjectPhoto?.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var fileName = (await FileService.UploadImageAsync(project.ProjectPhoto, uploadsFolder)).Result;
            model.ProjectPhotoUrl = fileName;
        }

        if (project.Id != null)
        {
            await _projectService.UpdateProject(model);

            ViewBag.Message = "Project updated successfully!";
            return RedirectToAction("Projects");
           
        }
        else
        {
            await _projectService.AddProject(model);
            ViewBag.Message = "Project added successfully!";
            return RedirectToAction("Projects");
        }
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
}
