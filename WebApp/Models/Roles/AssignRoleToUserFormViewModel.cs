using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Roles;

public class AssignRoleToUserFormViewModel
{
    [Required]
    [Display(Name = "Select User", Prompt ="Pick a user..")]
    public string UserId { get; set; } = null!;
    [Required]
    [Display(Name = "Select Role", Prompt = "Pick a role..")]
    public string RoleName { get; set; } = null!;
}
