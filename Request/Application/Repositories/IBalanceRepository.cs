using Request.Application.DTOs.Request;
using Request.Application.DTOs.Response;
using Request.Domain.Entities;
using Request.Domain.ValueObjects;
using Shared.Abstractions.Paging;

namespace Request.Application.Repositories;

public interface IBalanceRepository
{
    Task<PagedResult<BalancesResponse>> GetAllBalances(GetBalanceQuery? query = null);

    public Task<LeaveBalance> GetBalanceByUser(int userId, RequestType type, int? year = null);

    public Task<LeaveUser?> GetUserByUserName(string userName);

    public Task<LeaveUser> GetUserById(int userId);
}