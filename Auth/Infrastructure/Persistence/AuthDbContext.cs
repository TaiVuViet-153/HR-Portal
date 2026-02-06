using Microsoft.EntityFrameworkCore;
using Auth.Domain.Entities;

namespace Auth.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(e =>
        {
            e.ToTable("Users", "Auth");

            e.HasKey(x => x.UserID);

            e.Property(p => p.UserID).HasColumnName("UserID").ValueGeneratedOnAdd();
            e.Property(p => p.UserName).HasColumnName("UserName").IsRequired();
            e.Property(p => p.Email).HasColumnName("Email").IsRequired();
            e.Property(p => p.PasswordHash).HasColumnName("PasswordHash");
            e.Property(p => p.PasswordSalt).HasColumnName("PasswordSalt");
        });

        builder.Entity<Role>(e =>
        {
            e.ToTable("Roles", "Auth");

            e.HasKey(x => x.ID);

            e.Property(p => p.ID).HasColumnName("ID");
            e.Property(p => p.Code).HasColumnName("Code").IsRequired();
            e.Property(p => p.Name).HasColumnName("Name").IsRequired();
            e.Property(p => p.Description).HasColumnName("Description");
        });

        builder.Entity<Permission>(e =>
        {
            e.ToTable("Permissions", "Auth");

            e.HasKey(x => x.ID);

            e.Property(p => p.ID).HasColumnName("ID");
            e.Property(p => p.Code).HasColumnName("Code").IsRequired();
            e.Property(p => p.Name).HasColumnName("Name").IsRequired();
            e.Property(p => p.Description).HasColumnName("Description");
            e.Property(p => p.PolicyID).HasColumnName("Mnu_Id");
        });

        builder.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles", "Auth");

            e.HasKey(ur => new { ur.UserID, ur.RoleID });

            e.Property(p => p.UserID).HasColumnName("UserID").IsRequired();
            e.Property(p => p.RoleID).HasColumnName("RoleID").IsRequired();
        });

        builder.Entity<RolePermission>(e =>
        {
            e.ToTable("RolePermissions", "Auth");

            e.HasKey(rp => new { rp.RoleID, rp.PermissionID });

            e.Property(p => p.RoleID).HasColumnName("RoleID").IsRequired();
            e.Property(p => p.PermissionID).HasColumnName("PermissionID").IsRequired();
            e.Property(p => p.Policy).HasColumnName("Policy").IsRequired();
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens", "Auth");

            e.HasKey(x => x.ID);

            e.Property(p => p.ID).HasColumnName("ID");
            e.Property(p => p.UserID).HasColumnName("UserID");
            e.Property(p => p.ExpiresAt).HasColumnName("ExpiresAt");
            e.Property(p => p.CreatedByIP).HasColumnName("CreatedByIP");
            e.Property(p => p.CreatedDate).HasColumnName("CreatedDate");
        });
    }
}