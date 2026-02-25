using Request.Domain.ValueObjects;

namespace Request.Domain.Entities;

public sealed class LeaveBalance
{
    private const double MaxSingleAdjustment = 12.0; // maximum days allowed per update
    public int UserID { get; private set; }
    public RequestType Type { get; private set; }
    public int Year { get; private set; }
    public double Balance { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private LeaveBalance() { }

    public LeaveBalance(int userId, RequestType type, double balance)
    {
        UserID = userId;
        Type = type;
        Year = DateTime.UtcNow.Year;
        Balance = balance;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetBalance(double newBalance)
    {
        if (double.IsNaN(newBalance) || double.IsInfinity(newBalance)) return;
        if (newBalance < 0) return; // Prevent negative balance

        Balance = newBalance;
        UpdateNow();
    }

    public void DecreaseBalance(double amount)
    {
        // Validate amount
        if (double.IsNaN(amount) || double.IsInfinity(amount)) return;
        if (amount == 0d) return;

        // Do not allow single updates larger than allowed maximum
        if (Math.Abs(amount) > MaxSingleAdjustment) return;

        var newBalance = Balance - amount;
        if (newBalance < 0) return; // Prevent negative balance

        Balance = newBalance;
        UpdateNow();
    }

    public void UpdateNow()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasEnoughDays(double leaveDays)
    {
        return Balance >= leaveDays;
    }

    public bool IsNegativeBalance()
    {
        return Balance < 0;
    }
}