using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MansorySupplyHub.Implementation.Services
{
    public class ApplicationTypeService : IApplicationTypeService
    {
        private readonly ApplicationDbContext _dbcontext;

        public ApplicationTypeService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<ResponseModel<ApplicationTypeDto>> CreateApplicationType(CreateApplicationTypeDto request)
        {
            try
            {
                var applicationType = new ApplicationType
                {
                    Name = request.Name,
                };

                _dbcontext.ApplicationTypes.Add(applicationType);
                await _dbcontext.SaveChangesAsync();

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
                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while creating the ApplicationType."
                };
            }
        }

        public async Task<ResponseModel<ApplicationTypeDto>> EditApplicationType(UpdateApplicationTypeDto request, int id)
        {
            try
            {
                var applicationType = await _dbcontext.ApplicationTypes.FindAsync(id);
                if (applicationType == null)
                {
                    return new ResponseModel<ApplicationTypeDto>
                    {
                        Success = false,
                        Message = "ApplicationType not found"
                    };
                }

                applicationType.Name = request.Name;

                _dbcontext.ApplicationTypes.Update(applicationType);
                await _dbcontext.SaveChangesAsync();

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
                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the ApplicationType."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteApplicationType(int id)
        {
            try
            {
                var applicationType = await _dbcontext.ApplicationTypes.FindAsync(id);
                if (applicationType == null)
                {
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "ApplicationType not found"
                    };
                }

                _dbcontext.ApplicationTypes.Remove(applicationType);
                await _dbcontext.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "ApplicationType deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the ApplicationType."
                };
            }
        }

        public async Task<ResponseModel<List<ApplicationTypeDto>>> GetAllApplicationTypes()
        {
            try
            {
                var applicationTypes = await _dbcontext.ApplicationTypes.ToListAsync();
                var applicationTypeDtos = applicationTypes.Select(applicationType => new ApplicationTypeDto
                {
                    Id = applicationType.Id,
                    Name = applicationType.Name,
                }).ToList();

                return new ResponseModel<List<ApplicationTypeDto>>
                {
                    Data = applicationTypeDtos,
                    Success = true,
                    Message = "ApplicationTypes retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<ApplicationTypeDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving the ApplicationTypes."
                };
            }
        }

        public async Task<ResponseModel<ApplicationTypeDto>> GetApplicationTypeDetails(int id)
        {
            try
            {
                var applicationType = await _dbcontext.ApplicationTypes.FindAsync(id);
                if (applicationType == null)
                {
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

                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = applicationTypeDto,
                    Success = true,
                    Message = "ApplicationType details retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<ApplicationTypeDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving the ApplicationType details."
                };
            }
        }
    }
}
