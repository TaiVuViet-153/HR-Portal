namespace Request.Domain.ValueObjects;

public enum RequestType : byte
{
    Unpaid = 0,
    Paid = 1,
    Maternity = 2,
    Wedding = 3,
    Bereavement = 4
}