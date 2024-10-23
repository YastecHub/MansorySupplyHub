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
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext dbcontext, IWebHostEnvironment webHostEnvironment, ILogger<ProductService> logger)
        {
            _dbcontext = dbcontext;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<ResponseModel<ProductDto>> CreateProduct(CreateProductDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new product: {ProductName}", request.Name);

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

                _logger.LogInformation("Product created successfully: {ProductName}", request.Name);

                return new ResponseModel<ProductDto>
                {
                    Success = true,
                    Message = "Product created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product: {ProductName}", request.Name);

                return new ResponseModel<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the product."
                };
            }
            finally
            {
                _logger.LogDebug("CreateProduct method execution completed.");
            }
        }

        public async Task<ResponseModel<ProductDto>> EditProduct(UpdateProductDto request, int id)
        {
            try
            {
                _logger.LogInformation("Editing product: {ProductId}", id);

                var product = await _dbcontext.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found: {ProductId}", id);
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

                _logger.LogInformation("Product updated successfully: {ProductId}", id);

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
                _logger.LogError(ex, "An error occurred while updating the product: {ProductId}", id);

                return new ResponseModel<ProductDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while updating the product."
                };
            }
            finally
            {
                _logger.LogDebug("EditProduct method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product: {ProductId}", id);

                var product = await _dbcontext.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found: {ProductId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Product not found."
                    };
                }

                _dbcontext.Products.Remove(product);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully: {ProductId}", id);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Product deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product: {ProductId}", id);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the product."
                };
            }
            finally
            {
                _logger.LogDebug("DeleteProduct method execution completed.");
            }
        }

        public async Task<ResponseModel<List<ProductDto>>> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("Retrieving all products");

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

                _logger.LogInformation("Products retrieved successfully");

                return new ResponseModel<List<ProductDto>>
                {
                    Data = productDtos,
                    Success = true,
                    Message = "Products retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products");

                return new ResponseModel<List<ProductDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving products."
                };
            }
            finally
            {
                _logger.LogDebug("GetAllProducts method execution completed.");
            }
        }

        public async Task<ResponseModel<ProductDto>> GetProductDetails(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving product details: {ProductId}", id);

                var product = await _dbcontext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found: {ProductId}", id);
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

                _logger.LogInformation("Product details retrieved successfully: {ProductId}", id);

                return new ResponseModel<ProductDto>
                {
                    Data = productDto,
                    Success = true,
                    Message = "Product details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product details: {ProductId}", id);

                return new ResponseModel<ProductDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving product details."
                };
            }
            finally
            {
                _logger.LogDebug("GetProductDetails method execution completed.");
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectList()
        {
            try
            {
                _logger.LogInformation("Retrieving category select list");

                var categories = await _dbcontext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToListAsync();

                _logger.LogInformation("Category select list retrieved successfully");

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the category select list");

                return new List<SelectListItem>();
            }
            finally
            {
                _logger.LogDebug("GetCategorySelectList method execution completed.");
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetApplicationTypeSelectList()
        {
            try
            {
                _logger.LogInformation("Retrieving application type select list");

                var applicationTypes = await _dbcontext.ApplicationTypes.Select(at => new SelectListItem
                {
                    Text = at.Name,
                    Value = at.Id.ToString()
                }).ToListAsync();

                _logger.LogInformation("Application type select list retrieved successfully");

                return applicationTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the application type select list");

                return new List<SelectListItem>();
            }
            finally
            {
                _logger.LogDebug("GetApplicationTypeSelectList method execution completed.");
            }
        }

        public async Task<ResponseModel<ProductDto>> UpsertProduct(ProductDto productDto, IFormFileCollection files, string webRootPath)
        {
            var response = new ResponseModel<ProductDto>();

            // Ensure WC.ImagePath is relative (e.g., "Image/Product")
            string relativeImagePath = WC.ImagePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string uploadPath = Path.Combine(webRootPath, relativeImagePath); // Combines web root with relative image path

            _logger.LogInformation($"Web root path: {webRootPath}");
            _logger.LogInformation($"Upload path: {uploadPath}");

            try
            {
                // Ensure the upload directory exists
                if (!Directory.Exists(uploadPath))
                {
                    _logger.LogInformation($"Directory does not exist. Creating directory at: {uploadPath}");
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString();

                // If the product ID is 0, we are creating a new product
                if (productDto.Id == 0)
                {
                    _logger.LogInformation("Creating a new product.");

                    // Handle file upload if a file is provided
                    if (files.Count > 0)
                    {
                        string extension = Path.GetExtension(files[0].FileName);
                        string fullPath = Path.Combine(uploadPath, fileName + extension);

                        _logger.LogInformation($"Saving new file with filename: {fileName + extension} at {fullPath}");

                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            await files[0].CopyToAsync(fileStream);
                        }
                        productDto.Image = fileName + extension;
                    }

                    var createProductDto = new CreateProductDto
                    {
                        Name = productDto.Name,
                        Description = productDto.Description,
                        Price = productDto.Price,
                        Image = productDto.Image,
                        ApplicationType = productDto.ApplicationType,
                        Category = productDto.Category,
                        CategoryId = productDto.CategoryId,
                        ApplicationTypeId = productDto.ApplicationTypeId,
                    };

                    _logger.LogInformation("Calling CreateProduct method to add new product.");
                    response = await CreateProduct(createProductDto);
                }
                else // Updating an existing product
                {
                    _logger.LogInformation($"Updating product with ID: {productDto.Id}");

                    var objFromDb = await GetProductDetails(productDto.Id);
                    if (!objFromDb.Success)
                    {
                        _logger.LogWarning($"Product with ID {productDto.Id} not found.");
                        return new ResponseModel<ProductDto>
                        {
                            Success = false,
                            Message = "Product not found"
                        };
                    }

                    // Handle file upload if a new file is provided
                    if (files.Count > 0)
                    {
                        _logger.LogInformation("New file detected for update.");

                        var oldFilePath = Path.Combine(uploadPath, objFromDb.Data.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            _logger.LogInformation($"Deleting old file at: {oldFilePath}");
                            System.IO.File.Delete(oldFilePath);
                        }

                        string extension = Path.GetExtension(files[0].FileName);
                        string fullPath = Path.Combine(uploadPath, fileName + extension);

                        _logger.LogInformation($"Saving updated file with filename: {fileName + extension} at {fullPath}");

                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            await files[0].CopyToAsync(fileStream);
                        }
                        productDto.Image = fileName + extension;
                    }
                    else
                    {
                        // No new file uploaded, keep the old image
                        productDto.Image = objFromDb.Data.Image;
                        _logger.LogInformation("No new file uploaded. Keeping the old image.");
                    }

                    var updateProductDto = new UpdateProductDto
                    {
                        Id = productDto.Id,
                        Name = productDto.Name,
                        Description = productDto.Description,
                        Price = productDto.Price,
                        Image = productDto.Image,
                        Category = productDto.Category,
                        ApplicationType = productDto.ApplicationType,
                        CategoryId = productDto.CategoryId,
                        ApplicationTypeId = productDto.ApplicationTypeId,
                    };

                    _logger.LogInformation("Calling EditProduct method to update product.");
                    response = await EditProduct(updateProductDto, productDto.Id);
                }

                _logger.LogInformation("Product upsert successful.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while upserting the product with ID: {productDto.Id}");
                return new ResponseModel<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while processing the request."
                };
            }
        }


        public async Task<ResponseModel<ProductDto>> GetProductForUpsert(int? id)
        {
            if (id == null)
            {
                return new ResponseModel<ProductDto>
                {
                    Data = new ProductDto(),
                    Success = true,
                    Message = "New product creation"
                };
            }
            else
            {
                var response = await GetProductDetails(id.Value);
                return response;
            }
        }
    }
}
