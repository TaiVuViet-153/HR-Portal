namespace Shared.Abstractions.SuccessResponse;

public sealed record SuccessResponse<T>
(
    bool Success,
    string? Message,
    T? Data = default
);