using Employee.Domain.Entities;
using Employee.Domain.Specifications;
using Employee.Domain.ValueObjects;

namespace Employee.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
