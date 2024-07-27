using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IInquiryHeaderService
    {
        Task<ResponseModel<InquiryHeader>> GetInquiryHeaderByIdAsync(int id);
        Task<ResponseModel<IEnumerable<InquiryHeader>>> GetAllInquiryHeadersAsync();
        Task<ResponseModel<bool>> DeleteInquiryHeaderAsync(int id);
    }
}
