using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IEmailService
    {
        Task SendEmailClient(string msg, string title, string email);
        Task<BaseResponse<MailRecieverDto>> SendNotificationToUserAsync(Profile profile);
        Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request);

    }
}
