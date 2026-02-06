namespace Request.Application.DTOs;

public sealed record ApproveRequest
{
    public int RequestId { get; init; }
    public byte Status { get; init; }
}