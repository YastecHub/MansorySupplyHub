using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IInquiryDetailService
    {
        Task<ResponseModel<InquiryDetailDto>> CreateInquiryDetail(CreateInquiryDetailDto request);
        Task<ResponseModel<InquiryDetailDto>> EditInquiryDetail(UpdateInquiryDetailDto request);
        Task<ResponseModel<bool>> DeleteInquiryDetail(int id);
        Task<ResponseModel<List<InquiryDetailDto>>> GetInquiryDetailsByHeaderId(int inquiryHeaderId);
        Task<ResponseModel<bool>> DeleteInquiryDetailsByHeaderId(int inquiryHeaderId);

        Task<ResponseModel<List<ShoppingCart>>> ConvertInquiryToCart(int inquiryId);
    }
}
