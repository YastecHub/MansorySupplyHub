using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using static MansorySupplyHub.Implementation.Services.EmailService;

namespace MansorySupplyHub.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _hostenv;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly string _apiKey;

        public EmailService(IWebHostEnvironment hostenv, IOptions<EmailConfiguration> emailConfiguration, IConfiguration configuration)
        {
            _hostenv = hostenv;
            _emailConfiguration = emailConfiguration.Value;
            _apiKey = configuration.GetValue<string>("MailConfig:mailApikey");
        }

        public async Task<ResponseModel<MailReceiverDto>> SendNotificationToUserAsync(ApplicationUser user)
        {
            var mailReceiverRequestDto = new MailReceiverDto
            {
                Email = user.Email,
                Name = user.FullName + " " + user.UserName,
            };

            string emailBody = $"<p>Hello {user.FullName},</p>\r\n" +
                    $"<p>Welcome to Korede Hotel Management! We’re delighted to have you join our community.</p>\r\n" +
                    $"<p>At Korede Hotel, we are dedicated to providing exceptional service and a memorable experience for all our guests. Here are a few things you can expect:</p>\r\n" +
                    $"<ul>" +
                    $"<li><strong>Top-notch Accommodations:</strong> Enjoy our well-appointed rooms and suites designed for your comfort and relaxation.</li>" +
                    $"<li><strong>Exceptional Dining:</strong> Savor delicious meals at our on-site restaurant, offering a variety of culinary delights.</li>" +
                    $"<li><strong>Excellent Amenities:</strong> Take advantage of our state-of-the-art facilities, including a fitness center, spa, and swimming pool.</li>" +
                    $"<li><strong>Personalized Service:</strong> Our dedicated staff is here to ensure your stay is tailored to your needs.</li>" +
                    $"</ul>" +
                    $"<p>We hope you enjoy all that Korede Hotel has to offer and that your experience with us is nothing short of exceptional. Should you have any questions or require assistance, please do not hesitate to reach out to our team.</p>\r\n" +
                    $"<p>Once again, welcome to Korede Hotel! We look forward to serving you and making your stay unforgettable.</p>\r\n" +
                    $"<p>Warm regards,</p>\r\n" +
                    $"<p><strong>The Korede Hotel Team</strong></p>";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "WELCOME TO KOREDE HOTEL MANAGEMENT SYSTEM",
                HtmlContent = emailBody,
                ToEmail = "oseniahmadkorede@gmail.com"
            };

            try
            {
                await SendEmailAsync(mailReceiverRequestDto, mailRequest);
                return new ResponseModel<MailReceiverDto>
                {
                    Message = "Email sent successfully",
                    Success = true,
                    Data = mailReceiverRequestDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<MailReceiverDto>
                {
                    Message = $"Failed to send notification: {ex.Message}",
                    Success = false,
                };
            }
        }

        public async Task SendEmailClient(string msg, string title, string email)
        {
            if (string.IsNullOrEmpty(msg))
            {
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

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                Console.WriteLine("Inside email client");
                client.Connect(_emailConfiguration.SMTPServerAddress, _emailConfiguration.SMTPServerPort, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.EmailSenderAddress, _emailConfiguration.EmailSenderPassword);
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in email client: {ex.Message}");
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        public async Task<bool> SendEmailAsync(MailReceiverDto model, MailRequests request)
        {
            try
            {
                Console.WriteLine("Calling email client");
                string buildContent = $"Dear {model.Name},<p>{request.Body}</p>";

                if (string.IsNullOrWhiteSpace(request.HtmlContent))
                {
                    throw new ArgumentNullException(nameof(request.HtmlContent), "Email content cannot be null or empty");
                }

                await SendEmailClient(request.HtmlContent, request.Title, model.Email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending email: {ex.Message}");
                throw new Exception("There was an error while sending email", ex);
            }
        }
    }
}

