using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class AppUser : IdentityUser<Guid>
{
    public ICollection<ProjectEntity> Projects { get; set; } = [];

    public ProfileEntity Profile { get; set; } = null!;
}
