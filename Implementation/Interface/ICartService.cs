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
    }

}

