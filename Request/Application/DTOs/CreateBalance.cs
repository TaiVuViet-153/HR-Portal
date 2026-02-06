using Request.Domain.Entities;

namespace Request.Application.DTOs;

public sealed record CreateBalance
{
    public int UserID { get; init; }
    public byte Type { get; init; }
    public double Balance { get; init; }
}