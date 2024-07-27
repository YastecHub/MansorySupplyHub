using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IInquiryDetailService
    {
        Task<ResponseModel<IEnumerable<InquiryDetail>>> GetInquiryDetailsByHeaderIdAsync(Guid inquiryHeaderId);
        Task<ResponseModel<bool>> DeleteInquiryDetailsByHeaderIdAsync(Guid inquiryHeaderId);
    }
}
