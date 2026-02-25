using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Shared.Abstractions.Paging;

namespace Employee.Application.Repositories.Queries;

public interface IUserQueriesRepository
{
    Task<PagedResult<GetUserResponse>> GetAll(GetUserRequest? request);
    GetUserResponse? GetById(int userId);
}