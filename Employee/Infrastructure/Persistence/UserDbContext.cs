using Microsoft.EntityFrameworkCore;
using Employee.Domain.Entities;
using Employee.Application.Interfaces;

namespace Employee.Infrastructure.Persistence;

public class UserDbContext : DbContext, IUnitOfWork
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<LeaveBalance> LeaveBalances { get; set; } = null!;

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
            e.Property(p => p.PasswordHash).HasColumnName("PasswordHash").IsRequired();
            e.Property(p => p.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
            e.Property(p => p.Detail).HasColumnName("Detail");
            e.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
            e.Property(p => p.ModifiedDate).HasColumnName("ModifiedDate").IsRequired();
            e.Property(p => p.FailedLoginCount).HasColumnName("FailedLoginCount");
            e.Property(p => p.RequiredChangePW).HasColumnName("RequiredChangePW").IsRequired();
            e.Property(p => p.Status).HasColumnName("Status").IsRequired();
        });

        builder.Entity<Role>(e =>
        {
            e.ToTable("Roles", "Auth");

            e.HasKey(x => x.Id);

            e.Property(p => p.Id).HasColumnName("Id").ValueGeneratedOnAdd();
            e.Property(p => p.Name).HasColumnName("Name").IsRequired();
            e.Property(p => p.Code).HasColumnName("Code").IsRequired();
            e.Property(p => p.Description).HasColumnName("Description");
        });

        builder.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles", "Auth");

            e.HasKey(x => new { x.UserID, x.RoleID });

            e.Property(p => p.UserID).HasColumnName("UserID").IsRequired();
            e.Property(p => p.RoleID).HasColumnName("RoleID").IsRequired();
        });

        builder.Entity<LeaveBalance>(e =>
        {
            e.ToTable("LeaveBalances", "Management");

            e.HasKey(x => new { x.UserID, x.Type, x.Year });

            e.Property(p => p.UserID).HasColumnName("UserID").IsRequired();
            e.Property(p => p.Type).HasColumnName("Type").IsRequired();
            e.Property(p => p.Year).HasColumnName("Year").IsRequired();
            e.Property(p => p.Balance).HasColumnName("Balance").IsRequired();
        });
    }
}