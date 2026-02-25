using Request.Application.Mappings;
using Request.Application.Interfaces;
using Request.Domain.Repositories;
using Request.Application.DTOs.Response;
using Request.Application.DTOs.Request;
using Request.Application.Repositories;
using System.Text.Json.Nodes;
using Shared.Abstractions.Paging;
using Shared.Abstractions.SuccessResponse;
using Request.Domain.ValueObjects;

namespace Request.Application.Services;

public class BalanceService(
    ILeaveRepository leaveRepository,
    IBalanceRepository balanceRepository
) : IBalanceService
{
    public async Task<SuccessResponse<bool>> CreateBalance(CreateBalanceRequest newBalance)
    {
        var user = await balanceRepository.GetUserByUserName(newBalance.UserName);
        if (user == null)
            return new SuccessResponse<bool>(false, "User not found");

        var existingBalance = await balanceRepository.GetBalanceByUser(user.UserID, (RequestType)newBalance.Type, DateTime.UtcNow.Year);
        if (existingBalance != null)
            return new SuccessResponse<bool>(false, "Balance already exists for this user and leave type in " + DateTime.UtcNow.Year);

        var balance = BalanceMapper.ToEntity(newBalance, user.UserID);

        var insertResult = await leaveRepository.AddBalance(balance);
        if (insertResult == null)
            return new SuccessResponse<bool>(false, "Create balance failed");

        return new SuccessResponse<bool>(true, "Create balance succesfully");
    }

    public async Task<PagedResult<BalancesResponse>> GetAllAsync(GetBalanceQuery? request = null)
    {
        var pagedResult = await balanceRepository.GetAllBalances();

        var items = ParseUserListDetails(pagedResult.Items);

        return pagedResult.WithItems(items);
    }

    public async Task<SuccessResponse<bool>> UpdateBalance(UpdateBalanceRequest updateBalance)
    {
        var existingBalance = await balanceRepository.GetBalanceByUser(updateBalance.UserID, (RequestType)updateBalance.Type, updateBalance.Year);
        if (existingBalance == null)
            return new SuccessResponse<bool>(false, "Balance not found");

        var user = await balanceRepository.GetUserById(updateBalance.UserID);
        if (user == null)
            return new SuccessResponse<bool>(false, "User not found");

        if (updateBalance.Balance < 0)
            return new SuccessResponse<bool>(false, "Balance cannot be negative");

        existingBalance.SetBalance(updateBalance.Balance);

        var updateResult = await leaveRepository.UpdateBalance(existingBalance);
        if (updateResult == 0)
            return new SuccessResponse<bool>(false, "Update balance failed");

        return new SuccessResponse<bool>(true, "Update balance succesfully");
    }

    public async Task<SuccessResponse<bool>> DeleteBalance(int userId, byte type, int year)
    {
        var existingBalance = await balanceRepository.GetBalanceByUser(userId, (RequestType)type, year);
        if (existingBalance == null)
            return new SuccessResponse<bool>(false, "Balance not found");

        var user = await balanceRepository.GetUserById(userId);
        if (user == null)
            return new SuccessResponse<bool>(false, "User not found");

        var deleteResult = await leaveRepository.DeleteBalance(existingBalance);
        if (deleteResult == 0)
            return new SuccessResponse<bool>(false, "Delete balance failed");

        return new SuccessResponse<bool>(true, "Delete balance succesfully");
    }

    private List<BalancesResponse> ParseUserListDetails(IReadOnlyList<BalancesResponse> balances)
    {
        foreach (var balance in balances)
        {
            if (string.IsNullOrEmpty(balance.Detail)) continue;

            var detailObj = JsonNode.Parse(balance.Detail);

            if (detailObj == null) continue;

            balance.FirstName = detailObj["FirstName"]?.GetValue<string>();
            balance.LastName = detailObj["LastName"]?.GetValue<string>();
        }

        return balances.ToList();
    }

}