using Employee.Domain.ValueObjects;

namespace Employee.Application.DTOs;

public record UserDto(int Id, string UserDetail, DateTime CreatedAt);

public record CreateUserDto(string UserName, string Email);

public record UpdateUserDto(UserDetail UserDetail);

