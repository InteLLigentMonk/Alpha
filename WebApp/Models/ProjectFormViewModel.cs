using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace WebApp.Models;

public class ProjectFormViewModel : IValidatableObject
{
    public Guid? Id { get; set; }

    public IFormFile? ProjectPhoto { get; set; }
    public string? ProjectPhotoUrl { get; set; }

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Project Name", Prompt = "Project Name")]
    [RegularExpression(@"^.{3,}$", ErrorMessage = "Must be at least 3 characters long.")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Client Name", Prompt = "Client Name")]
    [RegularExpression(@"^.{3,}$", ErrorMessage = "Must be at least 3 characters long.")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Description", Prompt = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);


    [Required]
    public List<Member>? Members { get; set; } = [];


    [Required(ErrorMessage = "Required")]
    [Display(Name = "Budget")]
    [Range(0, double.MaxValue, ErrorMessage = "Must be a positive number")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Budget must be a number with maximum 2 decimal places")]
    public decimal Budget { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult(
                "End date must be after the start date",
                [nameof(EndDate)]);
        }
    }

    public static implicit operator Project(ProjectFormViewModel model)
    {

        var result = new Project
        {
            ProjectName = model.ProjectName,
            ClientName = model.ClientName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Users = model.Members,
            Budget = model.Budget,
            ProjectPhotoUrl = model.ProjectPhoto?.FileName
        };
        if (model.Id != null)
        {
            result.Id = model.Id.Value;
        }
        return result;

    }
}
