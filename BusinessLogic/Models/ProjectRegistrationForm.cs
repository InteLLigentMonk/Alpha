namespace BusinessLogic.Models;

public class ProjectRegistrationForm
{
    public Guid Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<String>? Members { get; set; } = [];
    public decimal Budget { get; set; }
    public string? ProjectPhotoUrl { get; set; }
}
