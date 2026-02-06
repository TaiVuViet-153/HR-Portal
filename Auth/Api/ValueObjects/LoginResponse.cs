namespace Auth.Api.ValueObjects;

public sealed record LoginResponse
{
    public string AccessToken { get; init; } = null!;
    public UserResponse User { get; init; } = null!;
    public bool IsRequiredChangePW { get; init; }
}