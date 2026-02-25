using Employee.Domain.Entities;

namespace Employee.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddRoleToCreatedUser(int userId);
    Task<int> SaveChangesAsync();
}
