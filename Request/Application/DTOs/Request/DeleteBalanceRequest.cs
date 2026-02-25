namespace Request.Application.DTOs.Request;

public sealed record DeleteBalanceRequest()
{
    public int UserID { get; init; }
    public byte Type { get; init; }
    public int Year { get; init; }
}