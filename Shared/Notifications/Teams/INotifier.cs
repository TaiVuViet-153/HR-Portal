namespace Shared.Notifications.Teams;

public interface INotifier
{
    Task SendAsync(string title, string message, CancellationToken ct = default);

    // Task SendAdaptiveAsync(string title, string message, CancellationToken ct = default);
}