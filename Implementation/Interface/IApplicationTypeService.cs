using MansorySupplyHub.Dto;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IApplicationTypeService
    {
        Task<ResponseModel<ApplicationTypeDto>> CreateApplicationType(CreateApplicationTypeDto request);
        Task<ResponseModel<ApplicationTypeDto>> EditApplicationType(UpdateApplicationTypeDto request, int id);
        Task<ResponseModel<bool>> DeleteApplicationType(int id);
        Task<ResponseModel<List<ApplicationTypeDto>>> GetAllApplicationTypes();
        Task<ResponseModel<ApplicationTypeDto>> GetApplicationTypeDetails(int id); 
    }
}
