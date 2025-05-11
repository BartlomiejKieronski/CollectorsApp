using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using CollectorsApp.Models;
using CollectorsApp.Services.Security;

namespace CollectorsApp.Services.Email
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IGoogleSecretStorageVault _vault;
        public EmailSenderService(IOptions<EmailSettings> emailSettings, IGoogleSecretStorageVault vault)
        {
            _emailSettings = emailSettings.Value;
            _vault = vault;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = htmlMessage };
            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSSL);

                if (!string.IsNullOrEmpty(_emailSettings.Username))
                {
                    await client.AuthenticateAsync(_emailSettings.Username, await _vault.GetSecretsAsync("EMAIL-DATA"));
                }

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {

            }
        }
    }
}