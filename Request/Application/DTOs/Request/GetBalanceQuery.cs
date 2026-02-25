using Shared.Abstractions.Paging;

namespace Request.Application.DTOs.Request;

public sealed class GetBalanceQuery : PagingQuery
{
    public string? Search { get; init; }
    public byte? Type { get; init; }
    public int? Year { get; init; }
}