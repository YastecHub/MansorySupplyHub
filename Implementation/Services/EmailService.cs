using System;
using System.Threading.Tasks;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace MansorySupplyHub.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IFluentEmail fluentEmail, ILogger<EmailService> logger)
        {
            _fluentEmail = fluentEmail;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email using the provided email metadata.
        /// </summary>
        /// <param name="emailMetadata">Metadata containing email details such as recipient, subject, body, and attachments.</param>
        public async Task Send(EmailMetadata emailMetadata)
        {
            try
            {
                var email = _fluentEmail
                    .To(emailMetadata.ToAddress)
                    .Subject(emailMetadata.Subject)
                    .Body(emailMetadata.Body, false); // true for HTML body

                if (!string.IsNullOrEmpty(emailMetadata.AttachmentPath))
                {
                    email.AttachFromFilename(emailMetadata.AttachmentPath);
                }

                await email.SendAsync();
                _logger.LogInformation("Email sent to {ToAddress} with subject {Subject}", emailMetadata.ToAddress, emailMetadata.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email to {ToAddress}", emailMetadata.ToAddress);
            }
        }
    }
}
