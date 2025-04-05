using Application.Common.Options;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services.SendGrid
{
    public class SendGridService(IOptions<SendGridOptions> options) : ISendGridService
    {
        private readonly SendGridOptions _options = options.Value;

        public async Task<bool> SendEmail(string[] adresses, string subject, string html)
        {
            if (string.IsNullOrEmpty(_options.Key) || adresses == null || adresses.Length == 0)
                return false;

            var client = new SendGridClient(_options.Key);

            var from = new EmailAddress(_options.From);
            var tos = adresses.Select(email => new EmailAddress(email)).ToList();

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, null, html);

            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}
