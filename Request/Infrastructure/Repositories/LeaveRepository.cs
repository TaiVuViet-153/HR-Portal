using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Request.Domain.Entities;
using Request.Domain.Repositories;
using Request.Infrastructure.Persistence;
using Request.Domain.ValueObjects;

namespace Request.Infrastructure.Repositories;

public class LeaveRepository(RequestDbContext _context) : ILeaveRepository
{
    public async Task<int> AddRequest(LeaveRequest newRequest)
    {
        _context.Requests.AddAsync(newRequest);

        return await SaveChangeAsync() == 1 ? newRequest.RequestId : 0;
    }
    public async Task<IEnumerable<string>?> GetByUserName(string userName)
    {
        var data = from r in _context.Requests.AsNoTracking()
                   join u in _context.Users.AsNoTracking() on r.UserID equals u.UserID
                   where u.UserName == userName
                   select new
                   {
                       r.RequestId,
                       r.Type,
                       r.StartDate,
                       r.EndDate,
                       r.IsHalfDayOff,
                       r.Reason,
                       r.Status,
                       u.UserName,
                       u.Email
                   };

        var responseData = new List<string>();

        foreach (var item in data)
        {
            responseData.Add(item?.ToString());
        }

        return responseData;
    }
    public async Task<LeaveRequest> GetByRequestId(int requestId)
    {
        return await _context.Requests.FirstOrDefaultAsync(x => x.RequestId == requestId);
    }
    public IQueryable<LeaveRequest> GetRequests()
    {
        return _context.Requests.AsNoTracking().Where(r => r.IsActive);
    }
    public RequestResponse GetUserNameForRequest(int requestId)
    {
        var data = from r in _context.Requests.AsNoTracking()
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

        return data.FirstOrDefault();
    }
    public async Task<LeaveBalance> GetBalanceByUser(int userId, RequestType type)
    {
        return await _context.Balances.FirstOrDefaultAsync(x => x.UserID == userId && x.Type == type);
    }
    public async Task<int> UpdateRequest(LeaveRequest updatedRequest)
    {
        _context.Requests.Update(updatedRequest);

        return await SaveChangeAsync() == 1 ? updatedRequest.RequestId : 0;
    }
    public async Task<int> AddBalance(LeaveBalance newBalance)
    {
        await _context.Balances.AddAsync(newBalance);

        return await SaveChangeAsync();
    }
    public async Task<IEnumerable<LeaveBalance>> GetAllBalanceByUser(int userId, RequestType type)
    {
        return await _context.Balances.Where(x => x.UserID == userId && x.Type == type).ToListAsync();
    }
    public async Task<int> UpdateBalance(LeaveBalance updatedBalance)
    {
        _context.Balances.Update(updatedBalance);

        return await SaveChangeAsync();
    }
    public async Task<int> SaveChangeAsync()
    {
        return await _context.SaveChangesAsync();
    }
}