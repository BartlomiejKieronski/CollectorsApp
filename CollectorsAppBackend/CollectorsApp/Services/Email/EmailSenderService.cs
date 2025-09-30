using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using CollectorsApp.Models;
using CollectorsApp.Services.Security;

namespace CollectorsApp.Services.Email
{
    /// <summary>
    /// Provides functionality for sending emails asynchronously using SMTP settings.
    /// </summary>
    /// <remarks>This service retrieves SMTP configuration from the provided <see cref="EmailSettings"/> and
    /// supports secure authentication using secrets stored in a <see cref="IGoogleSecretStorageVault"/>. It is designed
    /// to send HTML-formatted emails and supports SSL/TLS encryption for secure communication.</remarks>
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IGoogleSecretStorageVault _vault;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSenderService"/> class with the specified email settings,
        /// secret storage vault, and configuration.
        /// </summary>
        /// <param name="emailSettings">The email settings used to configure the email sender. This parameter cannot be null.</param>
        /// <param name="vault">The secret storage vault used to securely retrieve sensitive information. This parameter cannot be null.</param>
        /// <param name="configuration">The application configuration used to retrieve additional settings. This parameter cannot be null.</param>
        public EmailSenderService(IOptions<EmailSettings> emailSettings, IGoogleSecretStorageVault vault, IConfiguration configuration)
        {
            _emailSettings = emailSettings.Value;
            _vault = vault;
            _configuration = configuration;
        }

        /// <summary>
        /// Sends an email asynchronously using the specified recipient, subject, and HTML message content.
        /// </summary>
        /// <remarks>This method uses the SMTP protocol to send the email. Ensure that the SMTP server
        /// settings, including the sender's email address, are correctly configured in the application
        /// settings.</remarks>
        /// <param name="toEmail">The email address of the recipient. This parameter cannot be null or empty.</param>
        /// <param name="subject">The subject line of the email. This parameter cannot be null or empty.</param>
        /// <param name="htmlMessage">The HTML content of the email body. This parameter cannot be null or empty.</param>
        /// <returns></returns>
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
                    await client.AuthenticateAsync(_emailSettings.Username, await _vault.GetSecretsAsync(_configuration["GoogleSecretStorage:Secrets:EMAIL-DATA"]));
                }

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("failed to send email", ex);
            }
        }
    }
}