using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Employee.Application.Repositories.Queries;
using Employee.Domain.Entities;
using Employee.Domain.Repositories;
using Employee.Domain.ValueObjects;
using Employee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Request.Common.Paging;

namespace Employee.Infrastructure.Repositories;

public class UserRepository(
    UserDbContext _context
) : IUserRepository, IUserQueriesRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public IQueryable<User> GetAll(GetUserRequest? request)
    {
        var users = _context.Users.AsQueryable();
        Console.WriteLine($"Initial Users Count: {users.Count()}");
        users = ApplyFilter(users, request);
        users = ApplySorting(users, request);

        return users;
    }

    public IQueryable<GetUserResponse> IncludeRolesAndBalances(IQueryable<User> query)
    {
        var rolesGroup = from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleID equals r.Id
                         group r by ur.UserID into grouped
                         select new
                         {
                             UserID = grouped.Key,
                             Roles = grouped.Select(x => x.Code).Distinct().ToList()
                         };

        // var balancesGroup = from b in _context.LeaveBalances
        //                     group b by new { b.UserID, b.Type, b.Year } into grouped
        //                     select new
        //                     {
        //                         UserID = grouped.Key.UserID,
        //                         LeaveType = grouped.Key.Type,
        //                         SourceYear = grouped.Key.Year,
        //                         TotalBalance = grouped.Sum(x => x.Balance)
        //                     };

        var joinedQuery = from u in query
                          join rg in rolesGroup on u.UserID equals rg.UserID into userRoles
                          from rg in userRoles.DefaultIfEmpty()
                              //   join bg in balancesGroup on u.UserID equals bg.UserID into userBalances
                              //   from bg in userBalances.DefaultIfEmpty()
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
        Console.WriteLine($"Joined Query: {joinedQuery.Count()}");
        return joinedQuery;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
    }

    public IQueryable<User> GetById(int userId)
    {
        return _context.Users.Where(u => u.UserID == userId).AsQueryable();
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

    private IQueryable<User> ApplyFilter(IQueryable<User> data, GetUserRequest? request)
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

    private IQueryable<User> ApplySorting(IQueryable<User> query, GetUserRequest? request)
    {
        var page = request?.Page ?? PagingQuery.DefaultPage;
        var pageSize = request?.PageSize ?? PagingQuery.DefaultPageSize;

        if (page <= 0) page = PagingQuery.DefaultPage;
        if (pageSize <= 0) pageSize = PagingQuery.DefaultPageSize;

        // if (!string.IsNullOrWhiteSpace(request?.SortBy))
        // {
        //     query = request.IsAscending
        //         ? query.OrderBy(e => EF.Property<object>(e, request.SortBy))
        //         : query.OrderByDescending(e => EF.Property<object>(e, request.SortBy));
        // }
        // else
        // {
        //     query = request.IsAscending
        //         ? query.OrderBy(e => e.CreatedDate)
        //         : query.OrderByDescending(e => e.CreatedDate);
        // }

        if (!string.IsNullOrWhiteSpace(request?.SortBy))
        {
            Console.WriteLine($"Sorting By: {request.SortBy}, Ascending: {request.IsAscending}");
            query = request.SortBy.ToLower() switch
            {
                "username" => request.IsAscending
                    ? query.OrderBy(e => e.UserName)
                    : query.OrderByDescending(e => e.UserName),
                "email" => request.IsAscending
                    ? query.OrderBy(e => e.Email)
                    : query.OrderByDescending(e => e.Email),
                _ => request.IsAscending
                    ? query.OrderBy(e => e.CreatedDate)
                    : query.OrderByDescending(e => e.CreatedDate),
            };
        }

        return query;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
