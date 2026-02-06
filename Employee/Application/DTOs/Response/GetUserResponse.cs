using Employee.Domain.ValueObjects;

namespace Employee.Application.DTOs.Response;

public sealed class GetUserResponse
{
    public int UserID { get; init; }
    public string Email { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public UserStatus Status { get; init; }
    public List<string>? Roles { get; init; }
    public string? Detail { get; init; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BirthDate { get; set; }
    public List<BalancesResponse>? LeaveBalances { get; set; }
    public string? Address { get; set; }
    public string? Location { get; set; }
    public string? TimeZone { get; set; }
    public DateTime CreatedAt { get; init; }
}