using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbcontext;

        public CategoryService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<ResponseModel<CategoryDto>> CreateCategory(CreateCategoryDto request)
        {
            var category = new Category
            {
                Name = request.Name,
                DisplayOrder = request.DisplayOrder
            };

            _dbcontext.Categories.Add(category);
            await _dbcontext.SaveChangesAsync();

            var response = new ResponseModel<CategoryDto>
            {
                Data = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    DisplayOrder = category.DisplayOrder
                },
                Success = true,
                Message = "Category created successfully"
            };

            return response;
        }

        public async Task<ResponseModel<CategoryDto>> EditCategory(UpdateCategoryDto request, int id)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if (category == null)
            {
                return new ResponseModel<CategoryDto>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            category.Name = request.Name;
            category.DisplayOrder = request.DisplayOrder;

            _dbcontext.Categories.Update(category);
            await _dbcontext.SaveChangesAsync();

            var response = new ResponseModel<CategoryDto>
            {
                Data = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    DisplayOrder = category.DisplayOrder
                },
                Success = true,
                Message = "Category updated successfully"
            };

            return response;
        }

        public async Task<ResponseModel<bool>> DeleteCategory(int id)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if (category == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Category not found"
                };
            }

            _dbcontext.Categories.Remove(category);
            await _dbcontext.SaveChangesAsync();

            return new ResponseModel<bool>
            {
                Data = true,
                Success = true,
                Message = "Category deleted successfully"
            };
        }

        public async Task<ResponseModel<List<CategoryDto>>> GetAllCategories()
        {
            var categories = await _dbcontext.Categories.ToListAsync();
            var categoryDtos = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                DisplayOrder = category.DisplayOrder
            }).ToList();

            return new ResponseModel<List<CategoryDto>>
            {
                Data = categoryDtos,
                Success = true,
                Message = "Categories retrieved successfully"
            };
        }

        public async Task<ResponseModel<CategoryDto>> GetCategoryDetails(int id)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if (category == null)
            {
                return new ResponseModel<CategoryDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Category not found"
                };
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                DisplayOrder = category.DisplayOrder
            };

            return new ResponseModel<CategoryDto>
            {
                Data = categoryDto,
                Success = true,
                Message = "Category details retrieved successfully"
            };
        }
    }
}
