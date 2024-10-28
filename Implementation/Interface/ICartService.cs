using Braintree;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface ICartService
    {
        Task<ResponseModel<List<ProductDto>>> GetProductsInCart(List<int> productIds);
        Task<ResponseModel<ProductUserDto>> GetUserCartDetails(string userId, List<int> productIds);
        Task<ResponseModel<bool>> ClearCart(); 
        Task<ResponseModel<bool>> RemoveProductFromCart(int productId, List<ShoppingCart> shoppingCartList);

        Task<ResponseModel<List<ProductDto>>> GetCartProductsWithQuantities(HttpContext httpContext);

        Task UpdateSessionCart(IEnumerable<Product> products, HttpContext httpContext);

        Task<ResponseModel<ProductUserDto>> GetCartSummaryAsync(string userId);

       Task<ResponseModel<OrderHeaderDto>> CreateOrderAsync(ProductUserDto productUserDto, string userId, IEnumerable<ShoppingCart> shoppingCartList, string nonceFromTheClient);

       Task<ResponseModel<InquiryHeaderDto>> CreateInquiryAsync(ProductUserDto productUserDto, string userId, IEnumerable<ShoppingCart> shoppingCartList);
    }
}

