using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class HomeController(IWebHostEnvironment env) : Controller
{
    private readonly IWebHostEnvironment _env = env;

    public IActionResult Projects()
    {

        ViewData["Title"] = "Projects";
        return View(new ProjectsViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Projects(ProjectFormViewModel project)
    {
        if (!ModelState.IsValid || project.ProjectPhoto == null || project.ProjectPhoto.Length == 0)
            return View(new ProjectsViewModel { ProjectForm = project });

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{Path.GetFileName(project.ProjectPhoto.FileName)}");
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await project.ProjectPhoto.CopyToAsync(fileStream);
        }

        ViewBag.Message = "Project added successfully!";


        return RedirectToAction("Projects");
    }
    public IActionResult Members()
    {
        ViewData["Title"] = "Team Members";
        return View();
    }
}
