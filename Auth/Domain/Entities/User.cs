using System.Security.Cryptography;
using System.Text;

namespace Auth.Domain.Entities;

public sealed class User
{
    public int UserID { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public byte[] PasswordHash { get; private set; } = null!;
    public byte[] PasswordSalt { get; private set; } = null!;
    public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; private set; } = DateTime.UtcNow;
    public int? FailedLoginCount { get; set; }
    public readonly int MaxAttempts = 3;
    public UserStatus Status { get; set; }
    public bool? requiredChangePW { get; set; }
    public enum UserStatus : byte
    {
        Newly_Created = 1,
        Active = 2,
        Locked = 3
    }

    // other domain fields
    private User() { }
    public User(string userName, string email)
    {
        UserName = userName;
        Email = email;
    }

    public bool VerifyPassword(string inputPassword)
    {
        using var hmac = new HMACSHA512(PasswordSalt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
        return CryptographicOperations.FixedTimeEquals(computed, PasswordHash);
    }

    public void SetPassword(string inputPassword)
    {
        using var hmac = new HMACSHA512();
        PasswordSalt = hmac.Key;
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
    }
}
