using MansorySupplyHub.Dto;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IInquiryHeaderService
    {
        Task<ResponseModel<InquiryHeaderDto>> CreateInquiryHeader(CreateInquiryHeaderDto request);
        Task<ResponseModel<InquiryHeaderDto>> EditInquiryHeader(UpdateInquiryHeaderDto request, int id);
        Task<ResponseModel<bool>> DeleteInquiryHeader(int id);
        Task<ResponseModel<List<InquiryHeaderDto>>> GetAllInquiryHeaders();
        Task<ResponseModel<InquiryHeaderDto>> GetInquiryHeaderDetails(int id);
        Task<ResponseModel<InquiryVM>> GetInquiryDetailsVM(int id);
    }
}
