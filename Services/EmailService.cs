using System.Net;
using System.Net.Mail;

namespace MultiShop.Services
{
    public sealed class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string body, string subject, string receiver, bool isHtml = true)
        {
            SmtpClient smtpClient = new SmtpClient(_configuration["ApplicationEmail:Host"], Convert.ToInt32(_configuration["ApplicationEmail:Port"]));
            smtpClient.EnableSsl = true;

            smtpClient.Credentials = new NetworkCredential(_configuration["ApplicationEmail:Email"], _configuration["ApplicationEmail:Password"]);

            MailAddress from = new MailAddress(_configuration["ApplicationEmail:Email"], "MultiShop");
            MailAddress to = new MailAddress(receiver);

            MailMessage message = new MailMessage(from, to);
            message.IsBodyHtml = isHtml;
            message.Body = body;
            message.Subject = subject;
            
            await smtpClient.SendMailAsync(message);
        }
    }
}
