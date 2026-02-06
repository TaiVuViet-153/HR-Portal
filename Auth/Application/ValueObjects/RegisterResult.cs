namespace Auth.Application.ValueObjects;

public sealed record RegisterResult(bool registerSuccess, string errorMessage);