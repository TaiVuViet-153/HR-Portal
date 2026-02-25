namespace Auth.Application.ValueObjects;

public sealed record LoginResult(
    UserResult? UserResult,
    string errorMessage
);