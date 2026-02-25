namespace Request.Application.DTOs.Request;

public sealed record CreateBalanceRequest()
{
    public string? UserName { get; init; }
    public byte Type { get; init; }
    public double Balance { get; init; }
}