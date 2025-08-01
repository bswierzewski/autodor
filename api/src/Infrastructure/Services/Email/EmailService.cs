using Application.Common.Options;
using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services.Email
{
    public class EmailService(IOptions<EmailOptions> options) : INotificationService
    {
        private readonly EmailOptions _options = options.Value;

        public async Task<bool> Send(string[] addresses, string subject, string html)
        {
            if (string.IsNullOrEmpty(_options.Host) || addresses == null || addresses.Length == 0)
                return false;

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.From));

            foreach (var address in addresses)
            {
                email.To.Add(MailboxAddress.Parse(address));
            }

            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_options.Username, _options.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
    }
}
