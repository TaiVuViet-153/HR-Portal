using Request.Domain.ValueObjects;

namespace Request.Application.DTOs.Response;

public sealed record LeaveBalanceResponse()
{
    public RequestType LeaveType { get; init; }
    public int Year { get; init; }
    public double Balance { get; init; }
}