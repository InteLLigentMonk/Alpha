using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.JobTitles;

public class AddJobTitleFormViewModel
{
    [Required]
    [Display(Name = "Job Title", Prompt = "Enter your desired title..")]
    public string JobTitle { get; set; } = null!;
}
