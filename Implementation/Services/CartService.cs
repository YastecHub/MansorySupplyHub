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

        public async Task<ResponseModel<ProductUserDto>> GetUserCartDetails(string userId, List<int> productIds)
        {
            var user = await _dbContext.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
            var products = await GetProductsInCart(productIds);

            var userCartDetails = new ProductUserDto
            {
                ApplicationUser = new ApplicationUserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                },
                ProductList = products.Data
            };

            return new ResponseModel<ProductUserDto>
            {
                Data = userCartDetails,
                Success = true,
                Message = "User and cart details retrieved successfully."
            };
        }

        public async Task<ResponseModel<bool>> ClearCart() 
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

        public async Task<ResponseModel<bool>> RemoveProductFromCart(int productId, List<ShoppingCart> shoppingCartList)
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
    }
}
