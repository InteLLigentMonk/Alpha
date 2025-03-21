using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class LoginFormModel
{
    [Display(Name ="Email", Prompt ="Your email adress")]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Not Valid")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter your password")]
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}
