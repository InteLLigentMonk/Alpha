using Domain.Models;

namespace WebApp.Models;

public class ProjectsViewModel
{
    public IEnumerable<Member> Members { get; set; } = [];
    public IEnumerable<Project> Projects { get; set; } = [];
    public ProjectFormViewModel ProjectForm { get; set; } = new();
}
