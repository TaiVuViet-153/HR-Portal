namespace Auth.Domain.Entities;

public sealed class Role
{
    public Guid ID { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private Role() { } // EF Core

    public Role(string code, string name, string description)
    {
        ID = Guid.NewGuid();
        Code = code;
        Name = name;
        Description = description;
    }

    public void Update(string code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
    }
}