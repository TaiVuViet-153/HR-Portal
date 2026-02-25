namespace Request.Application.DTOs.Response;

public sealed record BalancesResponse()
{
    public int UserID { get; init; }
    public string? UserName { get; init; }
    public string? Detail { get; init; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public List<LeaveBalanceResponse>? LeaveBalances { get; init; }
    public DateTime CreatedAt { get; init; }
};