namespace Auth.Application.DTOs;

public sealed record RegisterRequest
{
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;

}