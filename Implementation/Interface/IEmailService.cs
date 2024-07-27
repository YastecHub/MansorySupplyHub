using MansorySupplyHub.Entities;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IEmailService
    {
        Task Send(EmailMetadata emailMetadata);
    }
}
