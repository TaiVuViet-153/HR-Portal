using Employee.Application.DTOs;
using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Request.Application.DTOs;
using Request.Common.Paging;

namespace Employee.Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<GetUserResponse>> GetAllAsync(GetUserRequest? request);
    Task<GetUserResponse> GetByIdAsync(int id);
    Task<UserResponse<CreateUserResponse>> CreateAsync(CreateUserRequest dto);
    Task<UserResponse<UpdateUserResponse>?> UpdateAsync(UpdateUserRequest dto);
    Task<UserResponse<bool>> DeleteAsync(int id);
    Task<UserResponse<string>> ResetPasswordAsync(int id);
    // Task<UserResponse<int?>> IncrementFailedLoginCountAsync(int id);
    Task<UserResponse<bool>> LockUserAsync(int id);
}