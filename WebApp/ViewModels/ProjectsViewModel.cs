using WebApp.Models;

namespace WebApp.ViewModels;

public class ProjectsViewModel
{
    public SearchFormModel SearchForm { get; set; } = new();
    public ProjectFormModel ProjectForm { get; set; } = new();
}
