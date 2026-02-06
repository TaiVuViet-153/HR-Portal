using Microsoft.EntityFrameworkCore;
using Request.Domain.Entities;

namespace Request.Infrastructure.Persistence;

public class RequestDbContext : DbContext
{
    public RequestDbContext(DbContextOptions<RequestDbContext> options) : base(options)
    {
    }

    public DbSet<LeaveRequest> Requests { get; set; } = null!;
    public DbSet<LeaveUser> Users { get; set; } = null!;
    public DbSet<LeaveBalance> Balances { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LeaveUser>(e =>
        {
            e.ToTable("Users", "Auth", tb => tb.ExcludeFromMigrations());

            e.HasNoKey();

            e.Property(p => p.UserID).HasColumnName("UserID");
            e.Property(p => p.UserName).HasColumnName("UserName");
            e.Property(p => p.Email).HasColumnName("Email");

        });

        builder.Entity<LeaveRequest>(e =>
        {
            e.ToTable("LeaveRequests", "Management");

            e.HasKey(x => x.RequestId);

            e.Property(p => p.RequestId).HasColumnName("RequestId").ValueGeneratedOnAdd();
            e.Property(p => p.UserID).HasColumnName("UserId").IsRequired();
            e.Property(p => p.Type).HasColumnName("Type").IsRequired();
            e.Property(p => p.StartDate).HasColumnName("StartDate").IsRequired();
            e.Property(p => p.EndDate).HasColumnName("EndDate").IsRequired();
            e.Property(p => p.IsHalfDayOff).HasColumnName("IsHalfDayOff");
            e.Property(p => p.Reason).HasColumnName("Reason");
            e.Property(p => p.CreatedAt).HasColumnName("CreatedAt");
            e.Property(p => p.UpdatedAt).HasColumnName("UpdatedAt");
            e.Property(p => p.Status).HasColumnName("Status");
            e.Property(p => p.IsActive).HasColumnName("IsActive").IsRequired();
        });

        builder.Entity<LeaveBalance>(e =>
        {
            e.ToTable("LeaveBalances", "Management");

            e.HasKey(x => new { x.UserID, x.Type, x.Year });

            e.Property(p => p.UserID).HasColumnName("UserId").IsRequired();
            e.Property(p => p.Type).HasColumnName("Type").IsRequired();
            e.Property(p => p.Year).HasColumnName("Year").IsRequired();
            e.Property(p => p.Balance).HasColumnName("Balance");
            e.Property(p => p.CreatedAt).HasColumnName("CreatedAt");
            e.Property(p => p.UpdatedAt).HasColumnName("UpdatedAt");
        });


    }
}