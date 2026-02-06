namespace Employee.Application.DTOs.Request;

public sealed class CreateUserRequest
{
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
}