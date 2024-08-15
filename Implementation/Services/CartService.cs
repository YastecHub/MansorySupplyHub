using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using MansorySupplyHub.Utility;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartService> _logger;
        private readonly IEmailService _emailService;

        public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<CartService> logger, IEmailService emailService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<ResponseModel<List<ProductDto>>> GetProductsInCart(List<int> productIds)
        {
            try
            {
                _logger.LogInformation("Retrieving products in cart for product IDs: {ProductIds}", productIds);

                var products = await _dbContext.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Description = p.Description,
                }).ToList();

                _logger.LogInformation("Products retrieved successfully.");

                return new ResponseModel<List<ProductDto>>
                {
                    Data = productDtos,
                    Success = true,
                    Message = "Products retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products in the cart.");

                return new ResponseModel<List<ProductDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving products in the cart."
                };
            }
            finally
            {
                _logger.LogDebug("GetProductsInCart method execution completed.");
            }
        }

        public async Task<ResponseModel<ProductUserDto>> GetUserCartDetails(string userId, List<int> productIds)
        {
            try
            {
                _logger.LogInformation("Retrieving user cart details for user ID: {UserId}", userId);

                var user = await _dbContext.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
                    return new ResponseModel<ProductUserDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "User not found."
                    };
                }

                var productsResponse = await GetProductsInCart(productIds);
                if (!productsResponse.Success)
                {
                    _logger.LogWarning("Failed to retrieve products for user ID: {UserId}", userId);
                    return new ResponseModel<ProductUserDto>
                    {
                        Data = null,
                        Success = false,
                        Message = productsResponse.Message
                    };
                }

                var userCartDetails = new ProductUserDto
                {
                    ApplicationUser = new ApplicationUserDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    },
                    ProductList = productsResponse.Data
                };

                _logger.LogInformation("User and cart details retrieved successfully for user ID: {UserId}", userId);

                return new ResponseModel<ProductUserDto>
                {
                    Data = userCartDetails,
                    Success = true,
                    Message = "User and cart details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user cart details for user ID: {UserId}", userId);

                return new ResponseModel<ProductUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving user cart details."
                };
            }
            finally
            {
                _logger.LogDebug("GetUserCartDetails method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> ClearCart()
        {
            try
            {
                _logger.LogInformation("Clearing the cart.");

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("HttpContext is null.");
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HttpContext is null."
                    };
                }

                httpContext.Session.Set(WC.SessionCart, new List<ShoppingCart>());

                _logger.LogInformation("Cart cleared successfully.");

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Cart cleared successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while clearing the cart.");

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while clearing the cart."
                };
            }
            finally
            {
                _logger.LogDebug("ClearCart method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> RemoveProductFromCart(int productId, List<ShoppingCart> shoppingCartList)
        {
            try
            {
                _logger.LogInformation("Removing product from cart: {ProductId}", productId);

                var productToRemove = shoppingCartList.FirstOrDefault(c => c.ProductId == productId);
                if (productToRemove != null)
                {
                    shoppingCartList.Remove(productToRemove);
                    _logger.LogInformation("Product removed from cart: {ProductId}", productId);

                    return new ResponseModel<bool>
                    {
                        Data = true,
                        Success = true,
                        Message = "Product removed from cart."
                    };
                }

                _logger.LogWarning("Product not found in cart: {ProductId}", productId);
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Product not found in cart."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the product from the cart: {ProductId}", productId);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while removing the product from the cart."
                };
            }
            finally
            {
                _logger.LogDebug("RemoveProductFromCart method execution completed.");
            }
        }
    }
}
