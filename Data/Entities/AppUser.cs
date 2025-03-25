using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class AppUser : IdentityUser<Guid>
{
    public IEnumerable<ProjectEntity> Projects { get; } = [];
}
