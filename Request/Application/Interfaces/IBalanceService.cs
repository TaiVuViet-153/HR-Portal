using Request.Application.DTOs.Request;
using Request.Application.DTOs.Response;
using Shared.Abstractions.Paging;
using Shared.Abstractions.SuccessResponse;

namespace Request.Application.Interfaces;

public interface IBalanceService
{
    Task<PagedResult<BalancesResponse>> GetAllAsync(GetBalanceQuery? request = null);
    Task<SuccessResponse<bool>> CreateBalance(CreateBalanceRequest newBalance);
    Task<SuccessResponse<bool>> UpdateBalance(UpdateBalanceRequest updateBalance);
    Task<SuccessResponse<bool>> DeleteBalance(int userId, byte type, int year);
}