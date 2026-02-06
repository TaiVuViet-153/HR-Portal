namespace Auth.Domain.ValueObjects;

public enum PolicyValue : byte
{
    NotSet = 0,
    Allow = 1,
    Inherit = 2
}