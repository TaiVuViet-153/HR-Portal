namespace Employee.Domain.Entities;

public sealed class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }

    private Role() { }

    public Role(string name, string code, string? description = null)
    {
        Name = name;
        Code = code;
        Description = description;
    }
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || Name == name)
            return;

        Name = name;
    }
    public void UpdateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || Code == code)
            return;

        Code = code;
    }
    public void UpdateDescription(string? description)
    {
        if (Description == description)
            return;

        Description = description;
    }
}
