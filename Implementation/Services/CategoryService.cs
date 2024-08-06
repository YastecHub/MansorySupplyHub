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
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext dbcontext, ILogger<CategoryService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<ResponseModel<CategoryDto>> CreateCategory(CreateCategoryDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new category: {CategoryName}", request.Name);

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

                _logger.LogInformation("Category created successfully: {CategoryId}", category.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the category: {CategoryName}", request.Name);
                return new ResponseModel<CategoryDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while creating the category."
                };
            }
            finally
            {
                _logger.LogDebug("CreateCategory method execution completed.");
            }
        }

        public async Task<ResponseModel<CategoryDto>> EditCategory(UpdateCategoryDto request, int id)
        {
            try
            {
                _logger.LogInformation("Editing category: {CategoryId}", id);

                var category = await _dbcontext.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", id);
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

                _logger.LogInformation("Category updated successfully: {CategoryId}", category.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the category: {CategoryId}", id);
                return new ResponseModel<CategoryDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the category."
                };
            }
            finally
            {
                _logger.LogDebug("EditCategory method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("Deleting category: {CategoryId}", id);

                var category = await _dbcontext.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Category not found"
                    };
                }

                _dbcontext.Categories.Remove(category);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Category deleted successfully: {CategoryId}", id);
                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Category deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the category: {CategoryId}", id);
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the category."
                };
            }
            finally
            {
                _logger.LogDebug("DeleteCategory method execution completed.");
            }
        }

        public async Task<ResponseModel<List<CategoryDto>>> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Retrieving all categories.");

                var categories = await _dbcontext.Categories.ToListAsync();
                var categoryDtos = categories.Select(category => new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    DisplayOrder = category.DisplayOrder
                }).ToList();

                _logger.LogInformation("Categories retrieved successfully.");

                return new ResponseModel<List<CategoryDto>>
                {
                    Data = categoryDtos,
                    Success = true,
                    Message = "Categories retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                return new ResponseModel<List<CategoryDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving categories."
                };
            }
            finally
            {
                _logger.LogDebug("GetAllCategories method execution completed.");
            }
        }

        public async Task<ResponseModel<CategoryDto>> GetCategoryDetails(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving details for category: {CategoryId}", id);

                var category = await _dbcontext.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", id);
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

                _logger.LogInformation("Category details retrieved successfully: {CategoryId}", id);
                return new ResponseModel<CategoryDto>
                {
                    Data = categoryDto,
                    Success = true,
                    Message = "Category details retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category details: {CategoryId}", id);
                return new ResponseModel<CategoryDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving category details."
                };
            }
            finally
            {
                _logger.LogDebug("GetCategoryDetails method execution completed.");
            }
        }
    }
}
