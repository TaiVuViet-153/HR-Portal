using Employee.Domain.ValueObjects;

namespace Employee.Application.DTOs.Response;

public sealed record BalancesResponse(
    RequestType LeaveType,
    int Year,
    double Balance
);