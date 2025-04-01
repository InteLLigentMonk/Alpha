using BusinessLogic.Models;
using Data.Entities;

namespace BusinessLogic.Factories;

public class ProjectFactory
{
    public static ProjectRegistrationForm ProjectRegistrationForm()
    {
        return new ProjectRegistrationForm
        {
            Id = new Guid()
        };
    }

    public static ProjectEntity ProjectEntity(ProjectRegistrationForm project)
    {
        return new ProjectEntity
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            ClientName = project.ClientName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ProjectPhotoUrl = project.ProjectPhotoUrl
        };
    }

}
