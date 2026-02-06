using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Employee.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Employee.Application.Repositories.Queries;

public interface IUserQueriesRepository
{
    IQueryable<User> GetAll(GetUserRequest? request);
    IQueryable<User> GetById(int userId);
    IQueryable<GetUserResponse> IncludeRolesAndBalances(IQueryable<User> query);
}