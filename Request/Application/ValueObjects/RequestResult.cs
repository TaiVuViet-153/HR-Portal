using Request.Domain.ValueObjects;

namespace Request.Application.ValueObjects;

public sealed record RequestResult(bool success, string? messeage, GetRequestResult? createdRequest = null);