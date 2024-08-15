using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class InquiryHeaderService : IInquiryHeaderService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<InquiryHeaderService> _logger;
        private readonly IEmailService _emailService;

        public InquiryHeaderService(ApplicationDbContext dbcontext, ILogger<InquiryHeaderService> logger, IEmailService emailService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<ResponseModel<InquiryHeaderDto>> CreateInquiryHeader(CreateInquiryHeaderDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new inquiry header for user: {UserId}", request.ApplicationUserId);

                if (string.IsNullOrEmpty(request.ApplicationUserId))
                {
                    _logger.LogWarning("ApplicationUserId is null or empty.");
                    return new ResponseModel<InquiryHeaderDto>
                    {
                        Success = false,
                        Message = "User ID is required."
                    };
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return new ResponseModel<InquiryHeaderDto>
                    {
                        Success = false,
                        Message = "Email is required."
                    };
                }

                var inquiryHeader = new InquiryHeader
                {
                    ApplicationUserId = request.ApplicationUserId,
                    InquiryDate = request.InquiryDate,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    Email = request.Email
                };

                _dbcontext.InquiryHeaders.Add(inquiryHeader);
                await _dbcontext.SaveChangesAsync();

                ////sending Email
                //var profile = new Profile
                //{
                //    Email = request.Email,
                //    FirstName = request.FullName
                //};

                //await _emailService.SendNotificationToUserAsync(profile);


                _logger.LogInformation("Inquiry header created successfully with ID: {InquiryHeaderId}", inquiryHeader.Id);

                var inquiryHeaderDto = new InquiryHeaderDto
                {
                    Id = inquiryHeader.Id,
                    ApplicationUserId = inquiryHeader.ApplicationUserId,
                    InquiryDate = inquiryHeader.InquiryDate,
                    PhoneNumber = inquiryHeader.PhoneNumber,
                    FullName = inquiryHeader.FullName,
                    Email = inquiryHeader.Email
                };

                return new ResponseModel<InquiryHeaderDto>
                {
                    Success = true,
                    Data = inquiryHeaderDto,
                    Message = "Inquiry header created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the inquiry header.");
                return new ResponseModel<InquiryHeaderDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the inquiry header."
                };
            }
            finally
            {
                _logger.LogDebug("CreateInquiryHeader method execution completed.");
            }
        }

        public async Task<ResponseModel<InquiryHeaderDto>> EditInquiryHeader(UpdateInquiryHeaderDto request, int id)
        {
            try
            {
                _logger.LogInformation("Editing inquiry header: {InquiryHeaderId}", id);

                var inquiryHeader = await _dbcontext.InquiryHeaders.FindAsync(id);
                if (inquiryHeader == null)
                {
                    _logger.LogWarning("Inquiry header not found: {InquiryHeaderId}", id);
                    return new ResponseModel<InquiryHeaderDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Inquiry header not found."
                    };
                }

                inquiryHeader.PhoneNumber = request.PhoneNumber;
                inquiryHeader.FullName = request.FullName;
                inquiryHeader.Email = request.Email;
                inquiryHeader.InquiryDate = request.InquiryDate;

                _dbcontext.InquiryHeaders.Update(inquiryHeader);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Inquiry header updated successfully: {InquiryHeaderId}", id);

                var inquiryHeaderDto = new InquiryHeaderDto
                {
                    Id = inquiryHeader.Id,
                    ApplicationUserId = inquiryHeader.ApplicationUserId,
                    InquiryDate = inquiryHeader.InquiryDate,
                    PhoneNumber = inquiryHeader.PhoneNumber,
                    FullName = inquiryHeader.FullName,
                    Email = inquiryHeader.Email
                };

                return new ResponseModel<InquiryHeaderDto>
                {
                    Data = inquiryHeaderDto,
                    Success = true,
                    Message = "Inquiry header updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the inquiry header: {InquiryHeaderId}", id);

                return new ResponseModel<InquiryHeaderDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the inquiry header."
                };
            }
            finally
            {
                _logger.LogDebug("EditInquiryHeader method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> DeleteInquiryHeader(int id)
        {
            try
            {
                _logger.LogInformation("Deleting inquiry header: {InquiryHeaderId}", id);

                var inquiryHeader = await _dbcontext.InquiryHeaders.FindAsync(id);
                if (inquiryHeader == null)
                {
                    _logger.LogWarning("Inquiry header not found: {InquiryHeaderId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Inquiry header not found."
                    };
                }

                _dbcontext.InquiryHeaders.Remove(inquiryHeader);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Inquiry header deleted successfully: {InquiryHeaderId}", id);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Inquiry header deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the inquiry header: {InquiryHeaderId}", id);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the inquiry header."
                };
            }
            finally
            {
                _logger.LogDebug("DeleteInquiryHeader method execution completed.");
            }
        }

        public async Task<ResponseModel<List<InquiryHeaderDto>>> GetAllInquiryHeaders()
        {
            try
            {
                _logger.LogInformation("Retrieving all inquiry headers.");

                var inquiryHeaders = await _dbcontext.InquiryHeaders.ToListAsync();

                var inquiryHeaderDtos = inquiryHeaders.Select(header => new InquiryHeaderDto
                {
                    Id = header.Id,
                    ApplicationUserId = header.ApplicationUserId,
                    InquiryDate = header.InquiryDate,
                    PhoneNumber = header.PhoneNumber,
                    FullName = header.FullName,
                    Email = header.Email
                }).ToList();

                _logger.LogInformation("Inquiry headers retrieved successfully.");

                return new ResponseModel<List<InquiryHeaderDto>>
                {
                    Data = inquiryHeaderDtos,
                    Success = true,
                    Message = "Inquiry headers retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving inquiry headers.");

                return new ResponseModel<List<InquiryHeaderDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving inquiry headers."
                };
            }
            finally
            {
                _logger.LogDebug("GetAllInquiryHeaders method execution completed.");
            }
        }

        public async Task<ResponseModel<InquiryHeaderDto>> GetInquiryHeaderDetails(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving inquiry header details: {InquiryHeaderId}", id);

                var inquiryHeader = await _dbcontext.InquiryHeaders.FindAsync(id);
                if (inquiryHeader == null)
                {
                    _logger.LogWarning("Inquiry header not found: {InquiryHeaderId}", id);
                    return new ResponseModel<InquiryHeaderDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Inquiry header not found."
                    };
                }

                var inquiryHeaderDto = new InquiryHeaderDto
                {
                    Id = inquiryHeader.Id,
                    ApplicationUserId = inquiryHeader.ApplicationUserId,
                    InquiryDate = inquiryHeader.InquiryDate,
                    PhoneNumber = inquiryHeader.PhoneNumber,
                    FullName = inquiryHeader.FullName,
                    Email = inquiryHeader.Email
                };

                _logger.LogInformation("Inquiry header details retrieved successfully: {InquiryHeaderId}", id);

                return new ResponseModel<InquiryHeaderDto>
                {
                    Data = inquiryHeaderDto,
                    Success = true,
                    Message = "Inquiry header details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving inquiry header details: {InquiryHeaderId}", id);

                return new ResponseModel<InquiryHeaderDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving inquiry header details."
                };
            }
            finally
            {
                _logger.LogDebug("GetInquiryHeaderDetails method execution completed.");
            }
        }
    }
}
