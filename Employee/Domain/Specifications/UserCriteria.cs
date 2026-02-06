namespace Employee.Domain.Specifications;

public sealed class UserCriteria
{
    public string? Search { get; init; }
    public int? Status { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
}