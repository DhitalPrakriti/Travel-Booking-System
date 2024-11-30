using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;  // Import MailKit.Security to use SecureSocketOptions

namespace TravelBookingSystem.Services
{
    public class EmailService  // This should be 'public class EmailService'
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Travel Booking System", _configuration["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var email = _configuration["EmailSettings:SenderEmail"];
                var password = _configuration["EmailSettings:SenderPassword"];

                // Attempt to parse the port number from configuration
                if (!int.TryParse(_configuration["EmailSettings:Port"], out var port))
                {
                    throw new ArgumentException("Invalid SMTP port specified in configuration.");
                }

                // For Gmail, port 587 should use STARTTLS (not SSL)
                var useSsl = false; // We are using STARTTLS, not SSL directly

                // Connect using STARTTLS
                client.Connect(smtpServer, port, SecureSocketOptions.StartTls);

                // Authenticate with the sender's email and app password
                client.Authenticate(email, password);

                // Send the email asynchronously
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Log the error (you could replace Console.WriteLine with ILogger)
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;  // Optionally re-throw the exception
            }
            finally
            {
                // Disconnect and clean up
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
