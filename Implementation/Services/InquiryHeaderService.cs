using MansorySupplyHub.Data;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MansorySupplyHub.Implementation.Services
{
    public class InquiryHeaderService : IInquiryHeaderService
    {
        private readonly ApplicationDbContext _dbContext;

        public InquiryHeaderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<InquiryHeader>> GetInquiryHeaderByIdAsync(int id)
        {
            try
            {
                var header = await _dbContext.InquiryHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (header == null)
                {
                    return new ResponseModel<InquiryHeader>
                    {
                        Success = false,
                        Message = "Inquiry Header not found."
                    };
                }

                return new ResponseModel<InquiryHeader>
                {
                    Data = header,
                    Success = true,
                    Message = "Inquiry Header retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                return new ResponseModel<InquiryHeader>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the inquiry header: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<InquiryHeader>>> GetAllInquiryHeadersAsync()
        {
            try
            {
                var headers = await _dbContext.InquiryHeaders
                    .AsNoTracking()
                    .ToListAsync();

                if (headers == null || headers.Count == 0)
                {
                    return new ResponseModel<IEnumerable<InquiryHeader>>
                    {
                        Success = false,
                        Message = "No inquiry headers found."
                    };
                }

                return new ResponseModel<IEnumerable<InquiryHeader>>
                {
                    Data = headers,
                    Success = true,
                    Message = "Inquiry Headers retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                return new ResponseModel<IEnumerable<InquiryHeader>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving inquiry headers: {ex.Message}"
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteInquiryHeaderAsync(int id)
        {
            try
            {
                var header = await _dbContext.InquiryHeaders.FindAsync(id);
                if (header == null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Message = "Inquiry Header not found.",
                        Data = false
                    };
                }

                _dbContext.InquiryHeaders.Remove(header);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Success = true,
                    Message = "Inquiry Header deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                return new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"An error occurred while deleting the inquiry header: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
