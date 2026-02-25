using Microsoft.EntityFrameworkCore;
using Request.Domain.Entities;
using Request.Domain.Repositories;
using Request.Infrastructure.Persistence;
using Request.Domain.ValueObjects;
using Request.Application.DTOs.Response;
using Request.Application.Repositories;
using Request.Application.DTOs.Request;
using Request.Application.Interfaces;
using Request.Application.DTOs.Query;
using Shared.Abstractions.Paging;
using Shared.Infrastructures.EFCore;

namespace Request.Infrastructure.Repositories;

public class LeaveRepository(
    RequestDbContext _context,
    IUnitOfWork _uow
) : ILeaveRepository, IBalanceRepository, IRequestRepository
{
    public async Task<int> AddRequest(LeaveRequest newRequest)
    {
        _context?.Requests.AddAsync(newRequest);

        return await SaveChangesAsync() == 1 ? newRequest.RequestId : 0;
    }

    public async Task<LeaveRequest?> GetRequestById(int requestId)
    {
        return await _context.Requests.FirstOrDefaultAsync(x => x.RequestId == requestId);
    }

    public async Task<PagedResult<RequestResponse>> GetRequests(GetRequestQuery query)
    {
        var data = from r in _context.Requests.AsNoTracking()
                   join u in _context.Users.AsNoTracking() on r.UserID equals u.UserID
                   where r.IsActive == true && (!query.UserID.HasValue || r.UserID == query.UserID.Value)
                   select new RequestJoinUser
                   {
                       Request = r,
                       User = u
                   };

        data = FilteredRequest(data, query);
        data = SortedRequest(data, query);

        return await PagingRequest(data, query);
    }

    public async Task<LeaveBalance> GetBalanceByUser(int userId, RequestType type, int? year = null)
    {
        if (year != null && year.HasValue)
        {
            return await _context.Balances.FirstOrDefaultAsync(x => x.UserID == userId && x.Type == type && x.Year == year.Value);
        }

        return await _context.Balances.FirstOrDefaultAsync(x => x.UserID == userId && x.Type == type);
    }

    public async Task<int> UpdateRequest(LeaveRequest updatedRequest)
    {
        _context.Requests.Update(updatedRequest);

        return await SaveChangesAsync() == 1 ? updatedRequest.RequestId : 0;
    }

    public async Task<LeaveBalance?> AddBalance(LeaveBalance newBalance)
    {
        _context?.Balances.AddAsync(newBalance);

        return await SaveChangesAsync() > 0 ? newBalance : null;
    }

    public async Task<PagedResult<BalancesResponse>> GetAllBalances(GetBalanceQuery? query = null)
    {
        var data = from b in _context.Balances.AsNoTracking()
                   join u in _context.Users.AsNoTracking() on b.UserID equals u.UserID
                   select new BalanceJoinUser
                   {
                       Balance = b,
                       User = u
                   };

        data = FilteredBalance(data, query);
        data = SortedBalance(data, query);

        return await PagingBalance(data, query);

    }

    public async Task<int> UpdateBalance(LeaveBalance updatedBalance)
    {
        Console.WriteLine($"Updating balance for UserID: {updatedBalance.UserID}, Type: {updatedBalance.Type}, Year: {updatedBalance.Year}, New Balance: {updatedBalance.Balance}");
        _context.Balances.Update(updatedBalance);

        return await SaveChangesAsync();
    }

    public async Task<int> DeleteBalance(LeaveBalance balanceToDelete)
    {
        _context.Balances.Remove(balanceToDelete);

        return await SaveChangesAsync();
    }

    public async Task<LeaveUser?> GetUserByUserName(string userName)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<LeaveUser> GetUserById(int userId)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == userId);
    }

    public async Task<RequestResponse?> GetUserByRequestId(int requestId)
    {
        var query = from r in _context.Requests.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on r.UserID equals u.UserID
                    where r.RequestId == requestId
                    select new RequestResponse
                    (
                        r.RequestId,
                        r.UserID,
                        u.UserName,
                        u.Email,
                        r.Type,
                        r.StartDate,
                        r.EndDate,
                        r.IsHalfDayOff,
                        r.Reason,
                        r.Status,
                        r.CreatedAt
                    );

        return await query.FirstOrDefaultAsync();
    }

    private IQueryable<RequestJoinUser> FilteredRequest(IQueryable<RequestJoinUser> data, GetRequestQuery query)
    {
        // ========== FILTER ==========
        if (!string.IsNullOrWhiteSpace(query.UserName))
            data = data.Where(x => x.User.UserName.Contains(query.UserName.Trim()));

        if (query.Type.HasValue)
            data = data.Where(x => x.Request.Type == (RequestType)query.Type.Value);

        if (query.Status.HasValue)
            data = data.Where(x => x.Request.Status == (RequestStatus)query.Status.Value);

        if (query.StartDate.HasValue)
            data = data.Where(x => x.Request.StartDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            data = data.Where(x => x.Request.EndDate <= query.EndDate.Value);

        if (query.IsHalfDayOff.HasValue)
            data = data.Where(x => x.Request.IsHalfDayOff == query.IsHalfDayOff);

        if (!string.IsNullOrWhiteSpace(query.Reason))
            data = data.Where(x => x.Request.Reason != null && x.Request.Reason.Contains(query.Reason));

        return data;
    }

    private IQueryable<RequestJoinUser> SortedRequest(IQueryable<RequestJoinUser> data, GetRequestQuery query)
    {
        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "CreatedAt" : query.SortBy;
        var ascending = query.SortDir == 1;

        return sortBy switch
        {
            "RequestId" => ascending ? data.OrderBy(x => x.Request.RequestId) : data.OrderByDescending(x => x.Request.RequestId),
            "Type" => ascending ? data.OrderBy(x => x.Request.Type) : data.OrderByDescending(x => x.Request.Type),
            "StartDate" => ascending ? data.OrderBy(x => x.Request.StartDate) : data.OrderByDescending(x => x.Request.StartDate),
            "EndDate" => ascending ? data.OrderBy(x => x.Request.EndDate) : data.OrderByDescending(x => x.Request.EndDate),
            _ => ascending ? data.OrderBy(x => x.Request.CreatedAt) : data.OrderByDescending(x => x.Request.CreatedAt)
        };

    }

    private IQueryable<RequestResponse> MappedRequest(IQueryable<RequestJoinUser> data)
    {
        return data.Select(x => new RequestResponse
        (
            x.Request.RequestId,
            x.Request.UserID,
            x.User.UserName,
            x.User.Email,
            x.Request.Type,
            x.Request.StartDate,
            x.Request.EndDate,
            x.Request.IsHalfDayOff,
            x.Request.Reason,
            x.Request.Status,
            x.Request.CreatedAt
        ));
    }

    private async Task<PagedResult<RequestResponse>> PagingRequest(IQueryable<RequestJoinUser> data, GetRequestQuery query)
    {
        var emptyResult = new PagedResult<RequestResponse>
        {
            Items = new List<RequestResponse>(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = 0
        };

        if (data == null || !data.Any())
            return emptyResult;

        var result = MappedRequest(data);

        return await result.ToPagedResultAsync(query);
    }

    private IQueryable<BalanceJoinUser> FilteredBalance(IQueryable<BalanceJoinUser> data, GetBalanceQuery? query)
    {
        if (query == null) return data;

        if (query.Type != null)
            data = data.Where(x => x.Balance.Type == (RequestType)query.Type);

        if (query.Year.HasValue)
            data = data.Where(x => x.Balance.Year == query.Year);

        if (!string.IsNullOrWhiteSpace(query.Search))
            data = data.Where(x => x.User.UserName.Contains(query.Search) || x.User.Email.Contains(query.Search));

        return data;
    }

    private IQueryable<BalanceJoinUser> SortedBalance(IQueryable<BalanceJoinUser> data, GetBalanceQuery? query)
    {
        if (query == null) return data.OrderByDescending(x => x.Balance.CreatedAt);

        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "createdat" : query.SortBy.ToLower();
        var ascending = query.SortDir == 1;

        return sortBy switch
        {
            "username" => ascending ? data.OrderBy(x => x.User.UserName) : data.OrderByDescending(x => x.User.UserName),
            "email" => ascending ? data.OrderBy(x => x.User.Email) : data.OrderByDescending(x => x.User.Email),
            "type" => ascending ? data.OrderBy(x => x.Balance.Type) : data.OrderByDescending(x => x.Balance.Type),
            "year" => ascending ? data.OrderBy(x => x.Balance.Year) : data.OrderByDescending(x => x.Balance.Year),
            _ => ascending ? data.OrderBy(x => x.Balance.CreatedAt) : data.OrderByDescending(x => x.Balance.CreatedAt)
        };
    }

    private List<BalancesResponse> MappedBalance(List<BalanceJoinUser> data)
    {
        return data
            .GroupBy(u => u.User.UserID)
            .Select(grouped => new BalancesResponse
            {
                UserID = grouped.First().User.UserID,
                UserName = grouped.First().User.UserName,
                Email = grouped.First().User.Email,
                Detail = grouped.First().User.Detail,
                LeaveBalances = grouped.Select(b => new LeaveBalanceResponse
                {
                    LeaveType = b.Balance.Type,
                    Year = b.Balance.Year,
                    Balance = b.Balance.Balance
                }).ToList(),
                CreatedAt = grouped.Min(b => b.Balance.CreatedAt)
            })
            .ToList();
    }

    private async Task<PagedResult<BalancesResponse>> PagingBalance(IQueryable<BalanceJoinUser> data, GetBalanceQuery? query)
    {
        if (data == null)
            return new PagedResult<BalancesResponse> { Items = [], Page = 1, PageSize = 20, TotalItems = 0 };

        return await data.ToPagedResultWithGroupingAsync(
            query,
            x => x.User.UserID,
            MappedBalance
        );
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _uow.SaveChangesAsync();
    }
}