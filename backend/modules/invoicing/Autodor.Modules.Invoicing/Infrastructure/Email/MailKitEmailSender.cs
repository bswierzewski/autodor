using Autodor.Modules.Invoicing.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Autodor.Modules.Invoicing.Infrastructure.Email;

public sealed class MailKitEmailSender(
    IOptions<SmtpOptions> options,
    ILogger<MailKitEmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Username))
        {
            logger.LogWarning("SMTP username is not configured – e-mail will not be sent");
            return;
        }

        var mime = new MimeMessage
        {
            Subject = message.Subject,
            Body = new TextPart("html") { Text = message.HtmlBody },
            From = { new MailboxAddress("Autodor", _options.Username) },
        };
        mime.To.AddRange(message.To.Select(MailboxAddress.Parse));

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_options.Username, _options.Password, ct);
        await client.SendAsync(mime, ct);
        await client.DisconnectAsync(quit: true, ct);

        logger.LogInformation("E-mail '{Subject}' sent to {Recipients}", message.Subject,
            string.Join(", ", message.To));
    }
}
