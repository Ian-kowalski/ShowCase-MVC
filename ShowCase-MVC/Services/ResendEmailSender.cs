using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ShowCase_MVC.Services;

public sealed class ResendEmailSender : IEmailSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailSender> _logger;

    public ResendEmailSender(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ResendEmailSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = _configuration["Resend:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Resend API key is missing. Confirmation email for {Email} was not sent.", email);
            return;
        }

        var from = _configuration["Resend:From"];
        if (string.IsNullOrWhiteSpace(from))
        {
            from = "onboarding@resend.dev";
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            from,
            to = new[] { email },
            subject,
            html = htmlMessage
        };

        try
        {
            using var response = await client.PostAsJsonAsync("https://api.resend.com/emails", payload);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Resend failed ({StatusCode}): {Body}", response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reach Resend API for {Email}", email);
        }
    }
}
