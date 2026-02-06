using Request.Application.DTOs;
using Request.Domain.Entities;
using Request.Domain.ValueObjects;

namespace Request.Application.Mappings;

public static class BalanceMapper
{
    public static LeaveBalance ToEntity(CreateBalance dto)
    {
        if (!Enum.IsDefined(typeof(RequestType), dto.Type))
            throw new ArgumentOutOfRangeException(nameof(dto.Type), $"Leave Type {dto.Type} invalid.");

        var type = (RequestType)dto.Type;

        return new LeaveBalance(
            userId: dto.UserID,
            type: type,
            balance: dto.Balance
        );
    }
}