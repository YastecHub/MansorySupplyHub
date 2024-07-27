using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IEmailService
    {
        Task SendEmailClient(string msg, string title, string email);
        Task<MailReceiverDto> SendNotificationToUserAsync(ApplicationUser profile);
        Task<bool> SendEmailAsync(MailReceiverDto model, MailRequests request);
    }
}
