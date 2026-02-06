namespace Auth.Api.ValueObjects;

public sealed record UserResponse
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Roles { get; init; } = null!;
}