namespace Employee.Application.DTOs.Response;

public sealed record UpdateUserResponse
{
    public int UserID { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string? Password { get; init; }
    public string? Detail { get; init; }
    public byte? Status { get; init; }
}