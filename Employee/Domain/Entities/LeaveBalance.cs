using Employee.Domain.ValueObjects;

namespace Employee.Domain.Entities;

public sealed class LeaveBalance
{
    public int UserID { get; private set; }
    public RequestType Type { get; private set; }
    public int Year { get; private set; }
    public double Balance { get; private set; }
    protected LeaveBalance() { }
    public byte ToParsedType()
    {
        return (byte)Type;
    }
}