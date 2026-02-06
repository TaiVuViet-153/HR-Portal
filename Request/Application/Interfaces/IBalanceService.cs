using Request.Application.DTOs;
using Request.Application.ValueObjects;

namespace Request.Application.Interfaces;

public interface IBalanceService
{
    Task<BalanceResult> CreateBalance(CreateBalance newBalance);
}