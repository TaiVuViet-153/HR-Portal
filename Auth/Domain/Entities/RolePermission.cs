namespace Auth.Domain.Entities;

public sealed class RolePermission
{
    public Guid RoleID { get; private set; }
    public Guid PermissionID { get; private set; }

    public byte Policy { get; private set; }

    private RolePermission() { }

    public RolePermission(Guid roleId, Guid permissionId, byte policy)
    {
        RoleID = roleId;
        PermissionID = permissionId;
        Policy = policy;
    }

    public void SetPolicy(byte policy) => Policy = policy;
}