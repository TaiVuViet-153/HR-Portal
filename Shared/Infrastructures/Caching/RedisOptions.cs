namespace Shared.Infrastructures.Caching;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "HRM:";
    public int DefaultExpirationMinutes { get; set; } = 30;
}
