using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SearchFormViewModel
{
    [Display(Name ="Search", Prompt="Search anything...")]
    [Required]
    public string? SearchText { get; set; }
}
