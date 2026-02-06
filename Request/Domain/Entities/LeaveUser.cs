namespace Request.Domain.Entities;

public sealed class LeaveUser
{
    public int UserID { get; init; }
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
}