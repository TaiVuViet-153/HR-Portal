namespace Auth.Application.DTOs;

public sealed record LoginRequest
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}