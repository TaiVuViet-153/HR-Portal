namespace Employee.Application.DTOs;

public sealed class EmailMessage
{
    public string From { get; init; } = null!;
    public IReadOnlyCollection<string> To { get; init; } = null!;
    public IReadOnlyCollection<string>? Cc { get; init; }
    public string Subject { get; init; } = null!;
    public string Html { get; init; } = null!;

}