using MansorySupplyHub.Dto;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface ICategoryService
    {
        Task<ResponseModel<CategoryDto>> CreateCategory(CreateCategoryDto request);
        Task<ResponseModel<CategoryDto>> EditCategory(UpdateCategoryDto request, int id);
        Task<ResponseModel<bool>> DeleteCategory(int id);
        Task<ResponseModel<List<CategoryDto>>> GetAllCategories();
        Task<ResponseModel<CategoryDto>> GetCategoryDetails(int id); 
    }
}
