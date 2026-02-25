using Request.Application.DTOs.Request;
using Request.Application.DTOs.Response;
using Request.Domain.Entities;
using Request.Domain.ValueObjects;

namespace Request.Application.Mappings;

public static class BalanceMapper
{
    public static LeaveBalance ToEntity(CreateBalanceRequest dto, int userId)
    {
        if (!Enum.IsDefined(typeof(RequestType), dto.Type))
            throw new ArgumentOutOfRangeException(nameof(dto.Type), $"Leave Type {dto.Type} invalid.");

        var type = (RequestType)dto.Type;

        return new LeaveBalance(
            userId: userId,
            type: type,
            balance: dto.Balance
        );
    }

    // public static CreateBalanceResponse ToCreateBalanceResponse(LeaveBalance entity, string? userName = null)
    // {
    //     return new CreateBalanceResponse()
    //     {
    //         UserID = entity.UserID,
    //         UserName = userName,
    //         Type = (byte)entity.Type,
    //         Year = entity.Year,
    //         Balance = entity.Balance
    //     };
    // }

    // public static UpdateBalanceResponse ToUpdateBalanceResponse(LeaveBalance entity, LeaveUser user)
    // {
    //     return new UpdateBalanceResponse()
    //     {
    //         UserID = entity.UserID,
    //         UserName = user.UserName,
    //         Type = (byte)entity.Type,
    //         Year = entity.Year,
    //         Balance = entity.Balance
    //     };
    // }
}