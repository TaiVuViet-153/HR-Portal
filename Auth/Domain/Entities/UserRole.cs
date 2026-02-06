namespace Auth.Domain.Entities;

public sealed class UserRole
{
    public int UserID { get; private set; }
    public Guid RoleID { get; private set; }

    private UserRole() { }

    public UserRole(int userId, Guid roleId)
    {
        UserID = userId;
        RoleID = roleId;
    }
}