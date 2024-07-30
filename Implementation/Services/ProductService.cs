using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(ApplicationDbContext dbcontext, IWebHostEnvironment webHostEnvironment)
        {
            _dbcontext = dbcontext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseModel<ProductDto>> CreateProduct(CreateProductDto request)
        {
            try
            {
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Image = request.Image,
                    Category = request.Category,
                    ApplicationType = request.ApplicationType,
                    CategoryId = request.CategoryId,
                    ApplicationTypeId = request.ApplicationTypeId,
                };

                _dbcontext.Products.Add(product);
                await _dbcontext.SaveChangesAsync();

                return new ResponseModel<ProductDto>
                {
                    Success = true,
                    Message = "Product created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the product."
                };
            }
        }

        public async Task<ResponseModel<ProductDto>> EditProduct(UpdateProductDto request, int id)
        {
            try
            {
                var product = await _dbcontext.Products.FindAsync(id);
                if (product == null)
                {
                    return new ResponseModel<ProductDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Product not found."
                    };
                }

                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Image = request.Image;

                _dbcontext.Products.Update(product);
                await _dbcontext.SaveChangesAsync();

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image,
                    Category = _dbcontext.Categories.FirstOrDefault(c => c.Id == product.CategoryId)
                };

                return new ResponseModel<ProductDto>
                {
                    Data = productDto,
                    Success = true,
                    Message = "Product updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<ProductDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the product."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteProduct(int id)
        {
            try
            {
                var product = await _dbcontext.Products.FindAsync(id);
                if (product == null)
                {
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Product not found."
                    };
                }

                _dbcontext.Products.Remove(product);
                await _dbcontext.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Product deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the product."
                };
            }
        }

        public async Task<ResponseModel<List<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _dbcontext.Products
                    .Include(p => p.Category)
                    .Include(p => p.ApplicationType)
                    .ToListAsync();

                var productDtos = products.Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image,
                    CategoryId = product.CategoryId,
                    Category = product.Category,
                    ApplicationTypeId = product.ApplicationTypeId,
                    ApplicationType = product.ApplicationType,
                }).ToList();

                return new ResponseModel<List<ProductDto>>
                {
                    Data = productDtos,
                    Success = true,
                    Message = "Products retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<ProductDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving products."
                };
            }
        }

        public async Task<ResponseModel<ProductDto>> GetProductDetails(int id)
        {
            try
            {
                var product = await _dbcontext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return new ResponseModel<ProductDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Product not found."
                    };
                }

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Image = product.Image,
                    Category = product.Category,
                    ApplicationType = product.ApplicationType,
                    CategoryId = product.CategoryId,
                    ApplicationTypeId = product.ApplicationTypeId,
                };

                return new ResponseModel<ProductDto>
                {
                    Data = productDto,
                    Success = true,
                    Message = "Product details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<ProductDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving product details."
                };
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectList()
        {
            try
            {
                return await _dbcontext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToListAsync();
            }
            catch (Exception ex)
            {
               
                return new List<SelectListItem>();
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetApplicationTypeSelectList()
        {
            try
            {
                return await _dbcontext.ApplicationTypes.Select(at => new SelectListItem
                {
                    Text = at.Name,
                    Value = at.Id.ToString()
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
        }

    }
}
