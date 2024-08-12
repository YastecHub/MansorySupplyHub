using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MansorySupplyHub.Dto;

namespace MansorySupplyHub.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _hostenv;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly string _apiKey;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IWebHostEnvironment hostenv, IOptions<EmailConfiguration> emailConfiguration, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _hostenv = hostenv;
            _emailConfiguration = emailConfiguration.Value;
            _apiKey = configuration.GetValue<string>("MailConfig:mailApikey");
            _logger = logger;
        }

        public async Task<BaseResponse<MailRecieverDto>> SendNotificationToUserAsync(Profile profile)
        {
            _logger.LogInformation("SendMessageToUserAsync called for user: {UserName}", profile.FirstName + " " + profile.LastName);

            var mailReceiverRequest = new MailRecieverDto
            {
                Email = profile.Email,
                Name = profile.FirstName + " " + profile.LastName,
            };

            string emailBody = $"<p>Hello {profile.FirstName},</p>\r\n" +
                 $"<p>Thank you for adding an item to your cart at Mansory Supply Hub!</p>\r\n" +
                 $"<p>We’re excited to let you know that your selection has been noted and is currently under review by our team.</p>\r\n" +
                 $"<p>What happens next?</p>\r\n" +
                 $"<ul>" +
                 $"<li><strong>Review Process:</strong> Our team will carefully review your selected item to ensure everything is in order.</li>" +
                 $"<li><strong>Confirmation:</strong> Once reviewed, you will receive a confirmation email with the next steps.</li>" +
                 $"<li><strong>Support:</strong> If you have any questions or need further assistance, feel free to reach out to our support team.</li>" +
                 $"</ul>" +
                 $"<p>We’re committed to providing you with the best service, and we appreciate your trust in us.</p>\r\n" +
                 $"<p>Thank you for choosing Mansory Supply Hub. We look forward to serving you!</p>\r\n" +
                 $"<p>Best regards,</p>\r\n" +
                 $"<p><strong>The Mansory Supply Hub Team</strong></p>";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "WELCOME TO YASIR MANSORYSUPPLYHUB",
                HtmlContent = emailBody,
                ToEmail = profile.Email 
            };


            try
            {
                await SendEmailAsync(mailReceiverRequest, mailRequest);
                _logger.LogInformation("Email sent successfully to: {Email}", mailReceiverRequest.Email);
                return new BaseResponse<MailRecieverDto>
                {
                    Message = "Email sent successfully",
                    Success = true,
                    Data = mailReceiverRequest
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {Email}", mailReceiverRequest.Email);
                return new BaseResponse<MailRecieverDto>
                {
                    Message = $"Failed to send notification: {ex.Message}",
                    Success = false,
                };
            }
        }

        public async Task SendEmailClient(string msg, string title, string email)
        {
            _logger.LogInformation("SendEmailClient called with email: {Email}, subject: {Title}", email, title);

            if (string.IsNullOrEmpty(msg))
            {
                _logger.LogError("Email message content cannot be null or empty");
                throw new ArgumentNullException(nameof(msg), "Email message content cannot be null or empty");
            }

            var message = new MimeMessage();
            message.To.Add(MailboxAddress.Parse(email));
            message.From.Add(new MailboxAddress(_emailConfiguration.EmailSenderName, _emailConfiguration.EmailSenderAddress));
            message.Subject = title;

            message.Body = new TextPart("html")
            {
                Text = msg
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    _logger.LogInformation("Connecting to SMTP server at {SMTPServerAddress}", _emailConfiguration.SMTPServerAddress);
                    client.Connect(_emailConfiguration.SMTPServerAddress, _emailConfiguration.SMTPServerPort, true);
                    client.Authenticate(_emailConfiguration.EmailSenderAddress, _emailConfiguration.EmailSenderPassword);
                    client.Send(message);
                    _logger.LogInformation("Email sent successfully to: {Email}", email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending email to: {Email}", email);
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public async Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request)
        {
            _logger.LogInformation("SendEmailAsync called for email: {Email}", model.Email);

            try
            {
                if (string.IsNullOrWhiteSpace(request.HtmlContent))
                {
                    _logger.LogError("Email content cannot be null or empty");
                    throw new ArgumentNullException(nameof(request.HtmlContent), "Email content cannot be null or empty");
                }

                await SendEmailClient(request.HtmlContent, request.Title, model.Email);
                _logger.LogInformation("Email content sent successfully to: {Email}", model.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email to: {Email}", model.Email);
                throw new Exception("There was an error while sending email", ex);
            }
        }


    }
}
