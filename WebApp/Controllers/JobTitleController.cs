using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.JobTitles;

namespace WebApp.Controllers;

public class JobTitleController(IJobTitleService jobTitleService) : Controller
{
    private readonly IJobTitleService _jobTitleService = jobTitleService;

    public async Task<IActionResult> JobTitlesAsync()
    {
        var response = await _jobTitleService.GetAllJobTitles();
        if (response.Succeeded && response.Result != null)
        {
            ViewData["JobTitles"] = response.Result;
            return View();
        }
        ViewBag.Error = response.Error;
        return View("Error");
    }

    public IActionResult Create()
    {
        return PartialView("Partials/_AddJobTitleFormPartial", new AddJobTitleFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddJobTitleFormViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var jobTitle = new JobTitleRegistrationFrom
            {
                Title = vm.JobTitle,
            };
            var response = await _jobTitleService.CreateJobTitle(jobTitle);
            if (response.Succeeded)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction("JobTitles");
            }
            ModelState.AddModelError("JobTitle", "Unable to create job title");
        }
        return PartialView("Partials/_AddJobTitleFormPartial", vm);
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _jobTitleService.Delete(id);
        if (response.Succeeded)
        {
            return RedirectToAction("JobTitles");
        }
        return NotFound();
    }
}
