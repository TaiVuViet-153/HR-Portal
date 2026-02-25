namespace Request.Application.DTOs.Request;

public sealed record UpdateBalanceRequest()
{
    public int UserID { get; init; }
    public byte Type { get; init; }
    public int Year { get; init; }
    public double Balance { get; init; }
}