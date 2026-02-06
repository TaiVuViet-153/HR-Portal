
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Shared.Notifications.Teams;

/// <summary>
/// Gửi thông báo tới Microsoft Teams bằng Incoming Webhook.
/// Đọc URL từ cấu hình: "Teams:WebhookUrl".
/// </summary>
public class TeamsNotifier : INotifier
{
    private readonly HttpClient _http;
    private readonly string _webhookUrl;

    // Khuyên dùng IHttpClientFactory để có handler cấu hình riêng (redirect, decompress)
    public TeamsNotifier(IHttpClientFactory factory, IConfiguration cfg)
    {
        _http = factory.CreateClient("teams-webhook");
        _webhookUrl = cfg["Teams:WebhookUrl"]
            ?? throw new InvalidOperationException("Missing Teams:WebhookUrl in configuration.");
    }

    /// <summary>
    /// Gửi thông điệp text đơn giản (schema tối thiểu yêu cầu 'text').
    /// </summary>
    public async Task SendAsync(string title, string message, CancellationToken ct = default)
    {
        var payload = new { text = $"{title}\n{message}" };

        var json = JsonSerializer.Serialize(payload);
        using var req = new HttpRequestMessage(HttpMethod.Post, _webhookUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var resp = await _http.SendAsync(req, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Teams webhook failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
        }
    }

    /// <summary>
    /// (Tuỳ chọn) Gửi Adaptive Card hiển thị đẹp hơn.
    /// </summary>

    // public async Task SendAdaptiveAsync(string title, string message, string email, string status, string detailUrl, CancellationToken ct = default)
    // {
    //     var card = new
    //     {
    //         type = "message",
    //         attachments = new[]
    //         {
    //         new {
    //             contentType = "application/vnd.microsoft.card.adaptive",
    //             content = new {
    //                 schema = "http://adaptivecards.io/schemas/adaptive-card.json",
    //                 type = "AdaptiveCard",
    //                 version = "1.4",
    //                 body = new object[]
    //                 {
    //                     new { type = "TextBlock", text = title, weight = "Bolder", size = "Large" },
    //                     new { type = "TextBlock", text = message, wrap = true, spacing = "Small" },
    //                     new {
    //                         type = "ColumnSet",
    //                         columns = new object[]
    //                         {
    //                             new {
    //                                 type = "Column",
    //                                 items = new object[]
    //                                 {
    //                                     new { type = "TextBlock", text = "Email", weight = "Bolder" },
    //                                     new { type = "TextBlock", text = "Trạng thái", weight = "Bolder" },
    //                                     new { type = "TextBlock", text = "Thời gian", weight = "Bolder" }
    //                                 },
    //                                 width = "auto"
    //                             },
    //                             new {
    //                                 type = "Column",
    //                                 items = new object[]
    //                                 {
    //                                     new { type = "TextBlock", text = email, wrap = true },
    //                                     new { type = "TextBlock", text = status, color = status.Contains("Thành công") ? "Good" : "Warning" },
    //                                     new { type = "TextBlock", text = DateTimeOffset.Now.ToString("dd/MM/yyyy HH:mm (zzz)"), wrap = true }
    //                                 },
    //                                 width = "stretch"
    //                             }
    //                         }
    //                     },
    //                     new { type = "TextBlock", text = "Ghi chú: Người dùng đã xác minh email.", wrap = true, spacing = "Medium", isSubtle = true }
    //                 },
    //                 actions = new object[]
    //                 {
    //                     new { type = "Action.OpenUrl", title = "Xem chi tiết", url = detailUrl },
    //                     new { type = "Action.OpenUrl", title = "Quản trị", url = "https://localhost:5127/admin" }
    //                 }
    //             }
    //         }
    //     }
    //     };

    //     var json = JsonSerializer.Serialize(card);
    //     using var req = new HttpRequestMessage(HttpMethod.Post, _webhookUrl.Trim())
    //     {
    //         Content = new StringContent(json, Encoding.UTF8, "application/json"),
    //         VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
    //     };
    //     using var resp = await _http.SendAsync(req, ct);
    //     if (!resp.IsSuccessStatusCode)
    //     {
    //         var body = await resp.Content.ReadAsStringAsync(ct);
    //         throw new HttpRequestException($"Teams webhook (adaptive) failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
    //     }
    // }



}
