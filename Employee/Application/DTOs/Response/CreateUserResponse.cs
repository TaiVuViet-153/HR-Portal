namespace Request.Application.DTOs;

public sealed record CreateUserResponse
{
    public int UserID { get; init; }
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}