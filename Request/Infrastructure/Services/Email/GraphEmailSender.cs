using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Request.Application.DTOs;
using Request.Application.Interfaces;

namespace Request.Infrastructure.Services.Email;

public sealed class GraphEmailSender : IEmailSender
{
    private readonly GraphServiceClient _graphClient;

    public GraphEmailSender(IOptions<GraphEmailOptions> options)
    {
        var opt = options.Value;
        Console.WriteLine("Initializing GraphEmailSender: " + opt.ClientId);

        var credential = new ClientSecretCredential(
            opt.TenantId,
            opt.ClientId,
            opt.ClientSecret);

        _graphClient = new GraphServiceClient(
            credential,
            new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        if (message.To == null || !message.To.Any())
            throw new ArgumentException("Email must have at least one recipient.");

        var graphMessage = new Message
        {
            Subject = message.Subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = message.Html
            },
        };

        graphMessage.ToRecipients = message.To
            .Select(email => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email
                }
            })
            .ToList();

        if (message.Cc != null && message.Cc.Any())
        {
            graphMessage.CcRecipients = message.Cc
                .Select(email => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = email
                    }
                })
                .ToList();
        }

        var body = new Microsoft.Graph
            .Users
            .Item
            .SendMail
            .SendMailPostRequestBody
        {
            Message = graphMessage,
            SaveToSentItems = true
        };

        await _graphClient
            .Users[message.From]
            .SendMail
            .PostAsync(body, cancellationToken: cancellationToken);
    }
}