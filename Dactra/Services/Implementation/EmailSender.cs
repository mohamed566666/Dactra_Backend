using Dactra.Models;
using Dactra.Services.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

namespace Dactra.Services.Implementation
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Dactra", _emailSettings.EmailFrom));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = body };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.EmailFrom, _emailSettings.Password);
            await smtp.SendAsync(emailMessage);
            await smtp.DisconnectAsync(true);
        }
    }
}
