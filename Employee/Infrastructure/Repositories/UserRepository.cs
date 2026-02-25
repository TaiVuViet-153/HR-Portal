using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Employee.Application.Interfaces;
using Employee.Application.Repositories.Queries;
using Employee.Domain.Entities;
using Employee.Domain.Repositories;
using Employee.Domain.ValueObjects;
using Employee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Paging;
using Shared.Infrastructures.EFCore;

namespace Employee.Infrastructure.Repositories;

public class UserRepository(
    UserDbContext _context,
    IUnitOfWork _uow
) : IUserRepository, IUserQueriesRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<PagedResult<GetUserResponse>> GetAll(GetUserRequest? request)
    {
        var users = _context.Users.AsQueryable();

        users = FilteredUser(users, request);
        users = SortedUser(users, request);

        var joinedQuery = IncludeRolesAndBalances(users);

        return await PagingUser(joinedQuery, request);
    }

    private IQueryable<GetUserResponse> IncludeRolesAndBalances(IQueryable<User> query)
    {
        var rolesGroup = from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleID equals r.Id
                         group r by ur.UserID into grouped
                         select new
                         {
                             UserID = grouped.Key,
                             Roles = grouped.Select(x => x.Code).Distinct().ToList()
                         };

        var joinedQuery = from u in query
                          join rg in rolesGroup on u.UserID equals rg.UserID into userRoles
                          from rg in userRoles.DefaultIfEmpty()
                          select new GetUserResponse
                          {
                              UserID = u.UserID,
                              UserName = u.UserName,
                              Status = (UserStatus)u.Status,
                              Email = u.Email,
                              Detail = u.Detail,
                              CreatedAt = u.CreatedDate,
                              Roles = rg.Roles,
                              LeaveBalances = _context.LeaveBalances
                                .Where(b => b.UserID == u.UserID)
                                .GroupBy(b => new { b.Type, b.Year })
                                .Select(g => new BalancesResponse(
                                    g.Key.Type,
                                    g.Key.Year,
                                    g.Sum(x => x.Balance)
                                ))
                                .ToList()
                          };

        return joinedQuery;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
    }

    public GetUserResponse? GetById(int userId)
    {
        var data = _context.Users.Where(u => u.UserID == userId).AsQueryable();

        var joinedQuery = IncludeRolesAndBalances(data);

        return joinedQuery.FirstOrDefault();
    }

    public async Task<int> CreateAsync(User user)
    {
        _context.Users.Add(user);

        return await SaveChangesAsync() > 0 ? user.UserID : 0;
    }

    public async Task<User?> UpdateAsync(User updatingUser)
    {
        _context.Users.Update(updatingUser);

        return await SaveChangesAsync() > 0 ? updatingUser : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        if (user == null) return false;

        user.MarkAsDeleted();

        return await SaveChangesAsync() > 0 ? true : false;
    }

    public async Task<bool> AddRoleToCreatedUser(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
        if (user == null) return false;

        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Code == "USER");
        if (defaultRole == null) return false;

        var userRole = new UserRole
        (
            userId,
            defaultRole.Id
        );

        _context.UserRoles.Add(userRole);

        return await SaveChangesAsync() > 0 ? true : false;
    }

    private IQueryable<User> FilteredUser(IQueryable<User> data, GetUserRequest? request)
    {
        if (request == null) return data;

        if (request.Status.HasValue)
            data = data.Where(x => x.Status == (UserStatus)request.Status.Value);

        if (request.CreatedAfter.HasValue)
            data = data.Where(x => x.CreatedDate >= request.CreatedAfter.Value);

        if (request.CreatedBefore.HasValue)
            data = data.Where(x => x.CreatedDate <= request.CreatedBefore.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            data = data.Where(x => x.UserName.Contains(request.Search) || x.Email.Contains(request.Search));

        return data;
    }

    private IQueryable<User> SortedUser(IQueryable<User> data, GetUserRequest? query)
    {
        if (query == null) return data.OrderByDescending(x => x.CreatedDate);

        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "createddate" : query.SortBy.ToLower();
        var ascending = query.SortDir == 1;

        return sortBy switch
        {
            "username" => ascending ? data.OrderBy(x => x.UserName) : data.OrderByDescending(x => x.UserName),
            "email" => ascending ? data.OrderBy(x => x.Email) : data.OrderByDescending(x => x.Email),
            _ => ascending ? data.OrderBy(x => x.CreatedDate) : data.OrderByDescending(x => x.CreatedDate)
        };
    }

    private async Task<PagedResult<GetUserResponse>> PagingUser(IQueryable<GetUserResponse> data, GetUserRequest? query)
    {
        var emptyResult = new PagedResult<GetUserResponse>
        {
            Items = new List<GetUserResponse>(),
            Page = query?.Page ?? 0,
            PageSize = query?.PageSize ?? 0,
            TotalItems = 0
        };

        if (data == null || !data.Any())
            return emptyResult;

        return await data.ToPagedResultAsync(query);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _uow.SaveChangesAsync();
    }
}
