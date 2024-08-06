using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class ApplicationTypeService : IApplicationTypeService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<ApplicationTypeService> _logger;

        public ApplicationTypeService(ApplicationDbContext dbcontext, ILogger<ApplicationTypeService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<ResponseModel<ApplicationTypeDto>> CreateApplicationType(CreateApplicationTypeDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new application type: {ApplicationTypeName}", request.Name);

                var applicationType = new ApplicationType
                {
                    Name = request.Name,
                };

                _dbcontext.ApplicationTypes.Add(applicationType);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Application type created successfully: {ApplicationTypeName}", request.Name);

                var response = new ResponseModel<ApplicationTypeDto>
                {
                    Data = new ApplicationTypeDto
                    {
                        Id = applicationType.Id,
                        Name = applicationType.Name,
                    },
                    Success = true,
                    Message = "ApplicationType created successfully"
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the application type: {ApplicationTypeName}", request.Name);

                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while creating the ApplicationType."
                };
            }
            finally
            {
                _logger.LogDebug("CreateApplicationType method execution completed.");
            }
        }

        public async Task<ResponseModel<ApplicationTypeDto>> EditApplicationType(UpdateApplicationTypeDto request, int id)
        {
            try
            {
                _logger.LogInformation("Editing application type: {ApplicationTypeId}", id);

                var applicationType = await _dbcontext.ApplicationTypes.FindAsync(id);
                if (applicationType == null)
                {
                    _logger.LogWarning("Application type not found: {ApplicationTypeId}", id);
                    return new ResponseModel<ApplicationTypeDto>
                    {
                        Success = false,
                        Message = "ApplicationType not found"
                    };
                }

                applicationType.Name = request.Name;

                _dbcontext.ApplicationTypes.Update(applicationType);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Application type updated successfully: {ApplicationTypeId}", id);

                var response = new ResponseModel<ApplicationTypeDto>
                {
                    Data = new ApplicationTypeDto
                    {
                        Id = applicationType.Id,
                        Name = applicationType.Name,
                    },
                    Success = true,
                    Message = "ApplicationType updated successfully"
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the application type: {ApplicationTypeId}", id);

                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the ApplicationType."
                };
            }
            finally
            {
                _logger.LogDebug("EditApplicationType method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> DeleteApplicationType(int id)
        {
            try
            {
                _logger.LogInformation("Deleting application type: {ApplicationTypeId}", id);

                var applicationType = await _dbcontext.ApplicationTypes.FindAsync(id);
                if (applicationType == null)
                {
                    _logger.LogWarning("Application type not found: {ApplicationTypeId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "ApplicationType not found"
                    };
                }

                _dbcontext.ApplicationTypes.Remove(applicationType);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Application type deleted successfully: {ApplicationTypeId}", id);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "ApplicationType deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the application type: {ApplicationTypeId}", id);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the ApplicationType."
                };
            }
            finally
            {
                _logger.LogDebug("DeleteApplicationType method execution completed.");
            }
        }

        public async Task<ResponseModel<List<ApplicationTypeDto>>> GetAllApplicationTypes()
        {
            try
            {
                _logger.LogInformation("Retrieving all application types");

                var applicationTypes = await _dbcontext.ApplicationTypes.ToListAsync();
                var applicationTypeDtos = applicationTypes.Select(applicationType => new ApplicationTypeDto
                {
                    Id = applicationType.Id,
                    Name = applicationType.Name,
                }).ToList();

                _logger.LogInformation("Application types retrieved successfully");

                return new ResponseModel<List<ApplicationTypeDto>>
                {
                    Data = applicationTypeDtos,
                    Success = true,
                    Message = "ApplicationTypes retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving application types");

                return new ResponseModel<List<ApplicationTypeDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving the ApplicationTypes."
                };
            }
            finally
            {
                _logger.LogDebug("GetAllApplicationTypes method execution completed.");
            }
        }

        public async Task<ResponseModel<ApplicationTypeDto>> GetApplicationTypeDetails(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving application type details: {ApplicationTypeId}", id);

                var applicationType = await _dbcontext.ApplicationTypes.FindAsync(id);
                if (applicationType == null)
                {
                    _logger.LogWarning("Application type not found: {ApplicationTypeId}", id);
                    return new ResponseModel<ApplicationTypeDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "ApplicationType not found"
                    };
                }

                var applicationTypeDto = new ApplicationTypeDto
                {
                    Id = applicationType.Id,
                    Name = applicationType.Name,
                };

                _logger.LogInformation("Application type details retrieved successfully: {ApplicationTypeId}", id);

                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = applicationTypeDto,
                    Success = true,
                    Message = "ApplicationType details retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving application type details: {ApplicationTypeId}", id);

                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving the ApplicationType details."
                };
            }
            finally
            {
                _logger.LogDebug("GetApplicationTypeDetails method execution completed.");
            }
        }
    }
}
