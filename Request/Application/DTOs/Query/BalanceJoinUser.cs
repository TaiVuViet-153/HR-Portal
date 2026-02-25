using Request.Domain.Entities;

namespace Request.Application.DTOs.Query;

public sealed class BalanceJoinUser
{
    public LeaveBalance Balance { get; set; } = null!;
    public LeaveUser User { get; set; } = null!;
}