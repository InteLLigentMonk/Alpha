using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ForgotPasswordFormViewModel
{
    [Display(Name = "Email", Prompt = "Your email adress")]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Not Valid")]
    public string Email { get; set; } = null!;
}
