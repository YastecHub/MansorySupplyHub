using System.Threading.Tasks;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using FluentEmail.Core;
using FluentEmail.Smtp;

namespace MansorySupplyHub.Implementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

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
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}
