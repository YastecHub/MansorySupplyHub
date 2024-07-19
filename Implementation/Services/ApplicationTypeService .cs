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

        public ApplicationTypeService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<ResponseModel<ApplicationTypeDto>> CreateApplicationType(CreateApplicationTypeDto request)
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

        public async Task<ResponseModel<ApplicationTypeDto>> EditApplicationType(UpdateApplicationTypeDto request, int id)
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

        public async Task<ResponseModel<bool>> DeleteApplicationType(int id)
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

        public async Task<ResponseModel<List<ApplicationTypeDto>>> GetAllApplicationTypes()
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
                Message = "ApplicationType retrieved successfully"
            };
        }

        public async Task<ResponseModel<ApplicationTypeDto>> GetApplicationTypeDetails(int id)
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
    }
}
