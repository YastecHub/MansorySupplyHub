using MansorySupplyHub.Data;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class InquiryDetailService : IInquiryDetailService
    {

        private readonly ApplicationDbContext _dbContext;

        public InquiryDetailService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<IEnumerable<InquiryDetail>>> GetInquiryDetailsByHeaderIdAsync(Guid inquiryHeaderId)
        {
            var details = await _dbContext.InquiryDetails
                .Include(u => u.Product)
                .Where(u => u.InquiryHeaderId == inquiryHeaderId)
                .ToListAsync();
            return new ResponseModel<IEnumerable<InquiryDetail>> { Data = details, Success = details != null, Message = details != null ? "Success" : "No Data" };
        }

        public async Task<ResponseModel<bool>> DeleteInquiryDetailsByHeaderIdAsync(Guid inquiryHeaderId)
        {
            var details = await _dbContext.InquiryDetails
                .Where(u => u.InquiryHeaderId == inquiryHeaderId)
                .ToListAsync();
            if (details == null || details.Count == 0)
                return new ResponseModel<bool> { Data = false, Success = false, Message = "Not Found" };

            _dbContext.InquiryDetails.RemoveRange(details);
            await _dbContext.SaveChangesAsync();
            return new ResponseModel<bool> { Data = true, Success = true, Message = "Deleted" };
        }

    }
}
