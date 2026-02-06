using Request.Application.DTOs;

namespace Request.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default
    );
}