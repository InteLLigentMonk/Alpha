using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Factories;

public class ProjectFactory(UserManager<AppUser> userManager)
{

    private readonly UserManager<AppUser> _userManager = userManager;

    public static Project Project()
    {
        return new Project
        {
            Id = new Guid()
        };
    }
    public static Project Project(ProjectEntity project)
    {
        return new Project
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            ClientName = project.ClientName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ProjectPhotoUrl = project.ProjectPhotoUrl,
            Users = project.Users.Select(u => new Member
            {
                UserId = u.Id
            }).ToList()
        };
    }

    public async Task<ProjectEntity> Project(Project project)
    {
        ProjectEntity projectEntity =  new()
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            ClientName = project.ClientName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ProjectPhotoUrl = project.ProjectPhotoUrl,
            Users = []
        };

        if (project.Users != null)
        {
            foreach (var user in project.Users)
            {
                if(user != null)
                {
                    var userResult = await _userManager.FindByIdAsync(user.UserId.ToString()!);
                    if (userResult != null)
                    {
                        projectEntity.Users.Add(userResult);
                    }
                }
            }
        }
        return projectEntity;
    }


}
