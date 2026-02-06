using Request.Application.DTOs;
using Request.Application.Mappings;
using Request.Application.Interfaces;
using Request.Application.ValueObjects;
using Request.Domain.Repositories;

namespace Request.Application.Services;

public class BalanceService(
    ILeaveRepository leaveRepository
) : IBalanceService
{
    public async Task<BalanceResult> CreateBalance(CreateBalance newBalance)
    {
        var balance = BalanceMapper.ToEntity(newBalance);

        var insertResult = await leaveRepository.AddBalance(balance);
        if (insertResult == 0)
            return new BalanceResult(false, "Create balance failed");

        return new BalanceResult(true, "Create balance succesfully");
    }
}