using System.Security.Cryptography;
using System.Text;
using Employee.Domain.ValueObjects;

namespace Employee.Domain.Entities;

public sealed class User
{
    public int UserID { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public byte[] PasswordHash { get; private set; } = null!;
    public byte[] PasswordSalt { get; private set; } = null!;
    public string? Detail { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime ModifiedDate { get; private set; }
    public UserStatus Status { get; private set; }
    public int? FailedLoginCount { get; private set; }
    public bool RequiredChangePW { get; private set; }

    private User() { }

    public User(string userName, string email, string inputPassword)
    {
        UserName = userName;
        Email = email;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        Status = UserStatus.Newly_Created;
        FailedLoginCount = 0;
        SetPassword(inputPassword);
    }

    public bool VerifyPassword(string inputPassword)
    {
        if (PasswordHash == null || PasswordSalt == null)
            throw new InvalidOperationException("User has no password");

        using var hmac = new HMACSHA512(PasswordSalt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
        return CryptographicOperations.FixedTimeEquals(computed, PasswordHash);
    }

    public void UpdateEmail(string newEmail)
    {
        if (Email == newEmail)
            return;

        Email = newEmail;
        UpdateNow();
    }

    public void SetPassword(string inputPassword)
    {
        using var hmac = new HMACSHA512();
        PasswordSalt = hmac.Key;
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
        UpdateNow();
    }

    public void UpdateStatus(UserStatus status)
    {
        if (!Enum.IsDefined(typeof(UserStatus), status) || Status == status)
            return;

        Status = status;
        UpdateNow();
    }

    public void ResetFailedLoginCount()
    {
        FailedLoginCount = 0;
        UpdateNow();
    }

    public void IncrementFailedLoginCount()
    {
        FailedLoginCount = (FailedLoginCount ?? 0) + 1;
        UpdateNow();
    }

    public void UpdateDetail(string detail)
    {
        Detail = detail;
        UpdateNow();
    }
    public void MarkAsDeleted()
    {
        Status = UserStatus.Deleted;
        UpdateNow();
    }

    public void SetRequiredChangePW(bool required)
    {
        RequiredChangePW = required;
        UpdateNow();
    }

    private void UpdateNow() => ModifiedDate = DateTime.UtcNow;
}