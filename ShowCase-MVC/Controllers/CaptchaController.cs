using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ShowCase_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public CaptchaController(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _http = factory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CaptchaRequest request)
        {
            string secret = _config["Recaptcha:SecretKey"]!;
            var res = await _http.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={request.Response}",
                null);
            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            double score = doc.RootElement.TryGetProperty("score", out var s) ? s.GetDouble() : 0;
            return Ok(new { score });
        }
    }

    public record CaptchaRequest(string Response);
}
