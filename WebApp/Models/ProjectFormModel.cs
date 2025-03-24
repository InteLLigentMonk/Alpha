using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ProjectFormModel
{
    public IFormFile? ProjectPhoto { get; set; }

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
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Required")]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
    [Required]
    public List<string>? Members { get; set; } = [];
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Budget")]
    [Range(0, double.MaxValue, ErrorMessage = "Must be a positive number")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Budget must be a number with maximum 2 decimal places")]
    public decimal Budget { get; set; }
}
