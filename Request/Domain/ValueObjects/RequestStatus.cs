namespace Request.Domain.Entities;

public enum RequestStatus : byte
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3,
    Deleted = 4
}