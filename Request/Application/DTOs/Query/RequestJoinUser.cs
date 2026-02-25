using Request.Domain.Entities;

namespace Request.Application.DTOs.Query;

public sealed class RequestJoinUser
{
    public LeaveRequest Request { get; set; } = null!;
    public LeaveUser User { get; set; } = null!;
}