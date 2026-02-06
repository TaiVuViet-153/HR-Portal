using Request.Domain.Entities;
using Request.Domain.ValueObjects;

namespace Request.Domain.Repositories;

public interface ILeaveRepository
{
    public IQueryable<LeaveRequest> GetRequests();
    public Task<LeaveRequest> GetByRequestId(int requestId);
    public RequestResponse GetUserNameForRequest(int requestId);
    public Task<IEnumerable<string>?> GetByUserName(string userName);
    public Task<int> AddRequest(LeaveRequest newRequest);
    public Task<int> UpdateRequest(LeaveRequest updatedRequest);
    public Task<IEnumerable<LeaveBalance>> GetAllBalanceByUser(int userId, RequestType type);
    public Task<LeaveBalance> GetBalanceByUser(int userId, RequestType type);
    public Task<int> AddBalance(LeaveBalance newBalance);
    public Task<int> UpdateBalance(LeaveBalance updatedBalance);
    public Task<int> SaveChangeAsync();
}