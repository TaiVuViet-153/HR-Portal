namespace Employee.Infrastructure.Services.Email;

public sealed class GraphEmailOptions
{
    public string TenantId { get; init; } = null!;
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
}