using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ShowCase_MVC.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public ContactController(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _http = factory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactRequest request)
        {
            string apiKey = _config["Resend:ApiKey"]!;
            string to = _config["Resend:To"]!;
            string from = _config["Resend:From"]!;

            var body = $"Van: {request.Firstname} {request.Lastname}\n" +
                       $"E-mail: {request.Email}\n" +
                       $"Telefoon: {request.Phone}\n\n" +
                       $"{request.Message}";

            var payload = JsonSerializer.Serialize(new
            {
                from,
                to = new[] { to },
                reply_to = new[] { $"{request.Firstname} {request.Lastname} <{request.Email}>" },
                subject = request.Subject,
                text = body
            });

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
                req.Headers.Add("Authorization", $"Bearer {apiKey}");
                req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var res = await _http.SendAsync(req);
                if (res.IsSuccessStatusCode)
                    return Ok(new { success = true, message = "Bericht verzonden" });

                var err = await res.Content.ReadAsStringAsync();
                return Ok(new { success = false, message = $"Resend fout: {err}" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }

    public record ContactRequest(string Email, string Subject, string Firstname, string Lastname, string Phone, string Message);
}
