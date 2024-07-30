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

        public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<List<ProductDto>>> GetProductsInCart(List<int> productIds)
        {
            try
            {
                var products = await _dbContext.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Description = p.Description,
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
                    Message = "An error occurred while retrieving products in the cart."
                };
            }
        }

        public async Task<ResponseModel<ProductUserDto>> GetUserCartDetails(string userId, List<int> productIds)
        {
            try
            {
                var user = await _dbContext.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
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

                return new ResponseModel<ProductUserDto>
                {
                    Data = userCartDetails,
                    Success = true,
                    Message = "User and cart details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<ProductUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving user cart details."
                };
            }
        }

        public async Task<ResponseModel<bool>> ClearCart()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HttpContext is null."
                    };
                }

                httpContext.Session.Set(WC.SessionCart, new List<ShoppingCart>());

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Cart cleared successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while clearing the cart."
                };
            }
        }

        public async Task<ResponseModel<bool>> RemoveProductFromCart(int productId, List<ShoppingCart> shoppingCartList)
        {
            try
            {
                var productToRemove = shoppingCartList.FirstOrDefault(c => c.ProductId == productId);
                if (productToRemove != null)
                {
                    shoppingCartList.Remove(productToRemove);
                    return new ResponseModel<bool>
                    {
                        Data = true,
                        Success = true,
                        Message = "Product removed from cart."
                    };
                }

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Product not found in cart."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while removing the product from the cart."
                };
            }
        }
    }
}
