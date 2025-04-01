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

        modelBuilder.Entity<ProfileEntity>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<ProfileEntity>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectEntity>(entity =>
        {
            entity.Property(e => e.Budget).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ProjectEntity>()
            .HasMany(p => p.Users)
            .WithMany(u => u.Projects)
            .UsingEntity(j => j.ToTable("UserProjects"));

    }

}
