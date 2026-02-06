using Employee.Application.DTOs;

namespace Employee.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default
    );
}