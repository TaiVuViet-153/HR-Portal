using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface IAuthRepository
{
    public Task<int> CreateUser(User newUser);
    public Task<int> UpdateUser(User userUpdate);
    public Task<int> HandleSuccessfulLogin(User userLogin);
    public Task<IEnumerable<string>> GetRoles(int userId);
    public Task<IEnumerable<string>> GetPermissions(int userId);
    public Task<User?> GetByUserNameAsync(string username);
    public Task<User?> GetByUserIdAsync(int userId);
    public Task<int> UpdateFailAttempts(User user);
    public Task<int> AddRefreshTokenAsync(int userId, string refreshToken);
    public Task<RefreshToken?> GetRefreshToken(string refreshToken);
    public Task<int> SaveChangesAsync();
    public Task<IEnumerable<string>> GetUsers();
}