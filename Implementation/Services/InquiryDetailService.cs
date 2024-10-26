using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class InquiryDetailService : IInquiryDetailService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<InquiryDetailService> _logger;
        private readonly IEmailService _emailService;

        public InquiryDetailService(ApplicationDbContext dbcontext, ILogger<InquiryDetailService> logger, IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<ResponseModel<InquiryDetailDto>> CreateInquiryDetail(CreateInquiryDetailDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new inquiry detail.");

                var inquiryDetail = new InquiryDetail
                {
                    InquiryHeaderId = request.InquiryHeaderId,
                    ProductId = request.ProductId
                };

                _dbcontext.InquiryDetails.Add(inquiryDetail);
                await _dbcontext.SaveChangesAsync();

                ////Sendinding Email
                //var inquiryHeader = await _dbcontext.InquiryHeaders.FindAsync(request.InquiryHeaderId);
                //if (inquiryHeader != null)
                //{
                //    var profile = new Profile
                //    {
                //        Email = inquiryHeader.Email,
                //        FirstName = inquiryHeader.FullName 
                //    };

                //    await _emailService.SendNotificationToUserAsync(profile);
                //}

                var inquiryDetailDto = new InquiryDetailDto
                {
                    Id = inquiryDetail.Id,
                    InquiryHeaderId = inquiryDetail.InquiryHeaderId,
                    ProductId = inquiryDetail.ProductId
                };

                _logger.LogInformation("Inquiry detail created successfully.");

                return new ResponseModel<InquiryDetailDto>
                {
                    Success = true,
                    Data = inquiryDetailDto,
                    Message = "Inquiry detail created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the inquiry detail.");

                return new ResponseModel<InquiryDetailDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the inquiry detail."
                };
            }
        }

        public async Task<ResponseModel<InquiryDetailDto>> EditInquiryDetail(UpdateInquiryDetailDto request)
        {
            try
            {
                _logger.LogInformation("Editing inquiry detail: {InquiryDetailId}", request.Id);

                var inquiryDetail = await _dbcontext.InquiryDetails.FindAsync(request.Id);
                if (inquiryDetail == null)
                {
                    _logger.LogWarning("Inquiry detail not found: {InquiryDetailId}", request.Id);
                    return new ResponseModel<InquiryDetailDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Inquiry detail not found."
                    };
                }

                inquiryDetail.InquiryHeaderId = request.InquiryHeaderId;
                inquiryDetail.ProductId = request.ProductId;

                _dbcontext.InquiryDetails.Update(inquiryDetail);
                await _dbcontext.SaveChangesAsync();

                var inquiryDetailDto = new InquiryDetailDto
                {
                    Id = inquiryDetail.Id,
                    InquiryHeaderId = inquiryDetail.InquiryHeaderId,
                    ProductId = inquiryDetail.ProductId
                };

                _logger.LogInformation("Inquiry detail updated successfully: {InquiryDetailId}", request.Id);

                return new ResponseModel<InquiryDetailDto>
                {
                    Success = true,
                    Data = inquiryDetailDto,
                    Message = "Inquiry detail updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the inquiry detail: {InquiryDetailId}", request.Id);

                return new ResponseModel<InquiryDetailDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the inquiry detail."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteInquiryDetail(int id)
        {
            try
            {
                _logger.LogInformation("Deleting inquiry detail: {InquiryDetailId}", id);

                var inquiryDetail = await _dbcontext.InquiryDetails.FindAsync(id);
                if (inquiryDetail == null)
                {
                    _logger.LogWarning("Inquiry detail not found: {InquiryDetailId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Inquiry detail not found."
                    };
                }

                _dbcontext.InquiryDetails.Remove(inquiryDetail);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Inquiry detail deleted successfully: {InquiryDetailId}", id);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Inquiry detail deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the inquiry detail: {InquiryDetailId}", id);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the inquiry detail."
                };
            }
        }

        public async Task<ResponseModel<List<InquiryDetailDto>>> GetInquiryDetailsByHeaderId(int inquiryHeaderId)
        {
            try
            {
                _logger.LogInformation("Retrieving inquiry details for header: {InquiryHeaderId}", inquiryHeaderId);

                var inquiryDetails = await _dbcontext.InquiryDetails
                    .Include(d => d.Product) // Include related Product data
                    .Where(d => d.InquiryHeaderId == inquiryHeaderId)
                    .ToListAsync();

                var inquiryDetailDtos = inquiryDetails.Select(detail => new InquiryDetailDto
                {
                    Id = detail.Id,
                    InquiryHeaderId = detail.InquiryHeaderId,
                    ProductId = detail.ProductId,
                    Product = detail.Product != null ? new ProductDto
                    {
                        Id = detail.Product.Id,
                        Name = detail.Product.Name,
                        Price = detail.Product.Price
                    } : null
                }).ToList();

                _logger.LogInformation("Inquiry details retrieved successfully for header: {InquiryHeaderId}", inquiryHeaderId);

                return new ResponseModel<List<InquiryDetailDto>>
                {
                    Data = inquiryDetailDtos,
                    Success = true,
                    Message = "Inquiry details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving inquiry details for header: {InquiryHeaderId}", inquiryHeaderId);

                return new ResponseModel<List<InquiryDetailDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving inquiry details."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteInquiryDetailsByHeaderId(int inquiryHeaderId)
        {
            try
            {
                _logger.LogInformation("Deleting inquiry details for header: {InquiryHeaderId}", inquiryHeaderId);

                var inquiryDetails = await _dbcontext.InquiryDetails
                    .Where(d => d.InquiryHeaderId == inquiryHeaderId)
                    .ToListAsync();

                if (inquiryDetails == null || !inquiryDetails.Any())
                {
                    _logger.LogWarning("No inquiry details found for header: {InquiryHeaderId}", inquiryHeaderId);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "No inquiry details found for the specified header."
                    };
                }

                _dbcontext.InquiryDetails.RemoveRange(inquiryDetails);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Inquiry details deleted successfully for header: {InquiryHeaderId}", inquiryHeaderId);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Inquiry details deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting inquiry details for header: {InquiryHeaderId}", inquiryHeaderId);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting inquiry details."
                };
            }
        }

        public async Task<ResponseModel<List<ShoppingCart>>> ConvertInquiryToCart(int inquiryId)
        {
            var inquiryDetailsResponse = await GetInquiryDetailsByHeaderId(inquiryId);

            if (!inquiryDetailsResponse.Success)
            {
                return new ResponseModel<List<ShoppingCart>>
                {
                    Success = false,
                    Message = "Unable to load inquiry details."
                };
            }

            var shoppingCartList = inquiryDetailsResponse.Data
                .Select(detail => new ShoppingCart
                {
                    ProductId = detail.ProductId,
                    Sqft = 1
                }).ToList();

            return new ResponseModel<List<ShoppingCart>>
            {
                Success = true,
                Data = shoppingCartList
            };
        }
    }
}
