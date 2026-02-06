namespace Employee.Domain.ValueObjects;

public enum UserStatus : byte
{
    Newly_Created = 1,
    Active = 2,
    Locked = 3,
    Deleted = 4
}