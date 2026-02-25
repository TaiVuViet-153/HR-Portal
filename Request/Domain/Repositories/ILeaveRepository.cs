using Request.Domain.Entities;

namespace Request.Domain.Repositories;

public interface ILeaveRepository
{

    public Task<int> AddRequest(LeaveRequest newRequest);
    public Task<int> UpdateRequest(LeaveRequest updatedRequest);
    public Task<LeaveBalance?> AddBalance(LeaveBalance newBalance);
    public Task<int> UpdateBalance(LeaveBalance updatedBalance);
    public Task<int> DeleteBalance(LeaveBalance balanceToDelete);
    public Task<int> SaveChangesAsync();
}