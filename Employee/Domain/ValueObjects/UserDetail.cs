namespace Employee.Domain.ValueObjects;

public sealed class UserDetail
{
    public string? Password { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? BirthDate { get; private set; }
    public string? Balance { get; private set; }
    public string? Address { get; private set; }
    public string? Location { get; private set; }
    public string? TimeZone { get; private set; }

    public UserDetail(
        string? password = null,
        string? firstName = null,
        string? lastName = null,
        string? phoneNumber = null,
        string? birthDate = null,
        string? balance = null,
        string? address = null,
        string? location = null,
        string? timeZone = null)
    {
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        Balance = balance;
        Address = address;
        Location = location;
        TimeZone = timeZone;
    }

    public UserDetail With(
        string? password = null,
        string? firstName = null,
        string? lastName = null,
        string? phoneNumber = null,
        string? birthDate = null,
        string? balance = null,
        string? address = null,
        string? location = null,
        string? timeZone = null)
    {
        return new UserDetail(
            password ?? Password,
            firstName ?? FirstName,
            lastName ?? LastName,
            phoneNumber ?? PhoneNumber,
            birthDate ?? BirthDate,
            balance ?? Balance,
            address ?? Address,
            location ?? Location,
            timeZone ?? TimeZone);
    }

    public Dictionary<string, string> ToDictionary()
    {
        var result = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(Password)) result["Password"] = Password;
        if (!string.IsNullOrEmpty(FirstName)) result["FirstName"] = FirstName;
        if (!string.IsNullOrEmpty(LastName)) result["LastName"] = LastName;
        if (!string.IsNullOrEmpty(PhoneNumber)) result["PhoneNumber"] = PhoneNumber;
        if (!string.IsNullOrEmpty(BirthDate)) result["BirthDate"] = BirthDate;
        if (!string.IsNullOrEmpty(Balance)) result["Balance"] = Balance;
        if (!string.IsNullOrEmpty(Address)) result["Address"] = Address;
        if (!string.IsNullOrEmpty(Location)) result["Location"] = Location;
        if (!string.IsNullOrEmpty(TimeZone)) result["TimeZone"] = TimeZone;

        return result;
    }

    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(ToDictionary());
    }

    public override string ToString() => ToJson();
}