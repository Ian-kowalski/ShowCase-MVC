using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace ShowCase_MVC.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ContactController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactRequest request)
        {
            var smtp = _config.GetSection("Smtp");
            string host = smtp["Host"]!;
            int port = int.Parse(smtp["Port"]!);
            string user = smtp["Username"]!;
            string pass = smtp["Password"]!;
            string to = smtp["To"]!;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{request.Firstname} {request.Lastname}", user));
            message.To.Add(new MailboxAddress("", to));
            message.ReplyTo.Add(new MailboxAddress($"{request.Firstname} {request.Lastname}", request.Email));
            message.Subject = request.Subject;
            message.Body = new TextPart("plain")
            {
                Text = $"Van: {request.Firstname} {request.Lastname}\n" +
                       $"E-mail: {request.Email}\n" +
                       $"Telefoon: {request.Phone}\n\n" +
                       $"{request.Message}"
            };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(user, pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return Ok(new { success = true, message = "Bericht verzonden" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }

    public record ContactRequest(string Email, string Subject, string Firstname, string Lastname, string Phone, string Message);
}
