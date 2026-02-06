using Employee.Domain.ValueObjects;

namespace Employee.Application.DTOs.Request;

public sealed record UpdateUserRequest
{
    public int UserID { get; init; }
    public string? Email { get; init; }
    public string? CurrentPassword { get; init; }
    public string? NewPassword { get; init; }
    public string? Detail { get; init; }
    public byte? Status { get; init; }
}