using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models;

namespace WebApp.Models;

public class RegistrationFormViewModel
{
    [Display(Name = "Email", Prompt = "Your email address")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage="Not valid")]
    [Required(ErrorMessage = "Required")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter your password")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_+\-])[A-Za-z\d@$!%*?&_+\-]{8,}$", ErrorMessage = "Must be at least 8 characters long, contain both lower & uppercase characters, contain at least one digit and one special character")]
    [Required(ErrorMessage ="Required")]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password", Prompt = "Confirm your password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Required")]
    [Compare("Password", ErrorMessage = "Don't match")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Accept Terms")]
    [Required(ErrorMessage = "Required")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Required")]
    public bool AcceptTerms { get; set; }

    public static implicit operator UserRegistrationForm(RegistrationFormViewModel model)
    {
        return model == null ? null! : new UserRegistrationForm
        {
            Email = model.Email
        };
    }
}
