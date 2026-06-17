using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace ShowCase_MVC.Controllers
{
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

            var body = $"Van: {request.Firstname} {request.Lastname}\n" +
                       $"E-mail: {request.Email}\n" +
                       $"Telefoon: {request.Phone}\n\n" +
                       $"{request.Message}";

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };
                var mail = new MailMessage(user, to, request.Subject, body);
                mail.ReplyToList.Add(new MailAddress(request.Email, $"{request.Firstname} {request.Lastname}"));
                await client.SendMailAsync(mail);
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
