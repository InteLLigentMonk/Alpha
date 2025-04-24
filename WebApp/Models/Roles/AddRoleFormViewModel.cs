using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace WebApp.Models.Roles;

public class AddRoleFormViewModel
{
    [Required]
    [Display(Name = "Role Name", Prompt ="Enter your desired role..")]
    public string Role { get; set; } = null!;
}
