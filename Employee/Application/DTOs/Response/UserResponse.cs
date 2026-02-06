namespace Employee.Application.DTOs.Response;

public sealed record UserResponse<T>(
    bool Success,
    string? Message,
    T? Data = default
);