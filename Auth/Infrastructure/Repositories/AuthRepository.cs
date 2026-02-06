using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class AuthRepository(AuthDbContext _context) : IAuthRepository
{
    public async Task<User?> GetByUserIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
    }

    public async Task<User?> GetByUserNameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<string>> GetPermissions(int userId)
    {
        var notset = (byte)PolicyValue.NotSet;
        var allow = (byte)PolicyValue.Allow;
        var inherit = (byte)PolicyValue.Inherit;

        var data = from ur in _context.UserRoles
                   join rp in _context.RolePermissions on ur.RoleID equals rp.RoleID
                   join p in _context.Permissions on rp.PermissionID equals p.ID
                   where ur.UserID == userId && rp.Policy != notset
                   select new
                   {
                       p.Code,
                       Policy = (PolicyValue)rp.Policy
                   };

        var list = new List<string>();

        foreach (var item in await data.ToListAsync())
        {
            list.Add($"{item.Code}.{allow}");

            if (item.Policy == PolicyValue.Inherit)
            {
                list.Add($"{item.Code}.{inherit}");
            }
        }

        return list;
    }

    public async Task<IEnumerable<string>> GetRoles(int userId)
    {
        var listRole = await (from ur in _context.UserRoles
                              join r in _context.Roles on ur.RoleID equals r.ID
                              where ur.UserID == userId
                              select r.Code).ToListAsync();

        return listRole;
    }

    public async Task<int> HandleSuccessfulLogin(User userLogin)
    {
        // Reset FailedLoginCount for successfully login user
        userLogin.FailedLoginCount = 0;
        // If user login succesfully we update some information
        return await UpdateUser(userLogin);
    }

    public async Task<int> UpdateUser(User userUpdate)
    {
        _context.Users.Update(userUpdate);

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateFailAttempts(User user)
    {
        var currentAttempts = user.FailedLoginCount ?? 0;
        if (currentAttempts < user.MaxAttempts)
        {
            currentAttempts++;
            user.FailedLoginCount = currentAttempts;
            var attemptsRemaining = user.MaxAttempts - currentAttempts;
            if (attemptsRemaining == 0)
            {
                //lock user
                user.Status = User.UserStatus.Locked;
            }
        }
        else
            user.FailedLoginCount = currentAttempts;

        await UpdateUser(user);

        return currentAttempts;
    }

    public async Task<int> AddRefreshTokenAsync(int userId, string refreshToken)
    {
        var rt = new RefreshToken(userId, refreshToken);

        await _context.RefreshTokens.AddAsync(rt);

        return await _context.SaveChangesAsync();
    }

    public async Task<int> CreateUser(User newUser)
    {
        await _context.Users.AddAsync(newUser);

        return await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshToken(string refreshToken)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
    }

    public async Task<IEnumerable<string>> GetUsers()
    {
        var data = await _context.Users.ToListAsync();

        var responseData = new List<string>();

        foreach (var item in data)
        {
            responseData.Add(item.ToString());
        }

        return responseData;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}