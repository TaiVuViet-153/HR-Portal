namespace Auth.Application.ValueObjects;

public sealed record UserResult(
    int UserId,
    string Username,
    string Roles,
    string AccessToken,
    string RefreshToken,
    bool? requiredChangePW
);
