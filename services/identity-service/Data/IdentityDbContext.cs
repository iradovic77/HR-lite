using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("hr_identity");

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Admin" },
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "HR" },
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Manager" },
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Employee" }
        );
    }
}
