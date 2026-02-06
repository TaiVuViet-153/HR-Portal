namespace Auth.Domain.Entities;

public sealed class RefreshToken
{
    public Guid ID { get; private set; }
    public int UserID { get; private set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByIP { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid id, int userId)
    {
        ID = id;
        UserID = userId;
        CreatedDate = DateTime.UtcNow;
    }

    public RefreshToken(int userId, string token, string? createdByIp = null)
    {
        ID = Guid.NewGuid();
        UserID = userId;
        Token = token;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
        CreatedDate = DateTime.UtcNow;
        CreatedByIP = createdByIp;
    }

    public void RevokeToken()
    {
        IsRevoked = true;
    }
}