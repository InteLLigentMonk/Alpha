using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{

    public DbSet<ProfileEntity> Profiles { get; set; } = null!;
    public DbSet<ProjectEntity> Projects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // One to One Relationship between User and Profile
        modelBuilder.Entity<ProfileEntity>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<ProfileEntity>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
