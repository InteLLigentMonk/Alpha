using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models;

public class ProjectRegistrationForm
{
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
    public List<string>? Members { get; set; } = [];
    public decimal Budget { get; set; }
    public string? ProjectPhotoUrl { get; set; }
}
