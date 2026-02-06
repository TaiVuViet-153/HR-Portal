namespace Auth.Domain.Entities;

public sealed class Permission
{
    public Guid ID { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int? PolicyID { get; private set; }

    private Permission() { }
    public Permission(string code, string name, string description, int policyId)
    {
        ID = Guid.NewGuid();
        Code = code;
        Name = name;
        Description = description;
        PolicyID = policyId;
    }

}