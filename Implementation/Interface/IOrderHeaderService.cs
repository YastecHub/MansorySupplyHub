using MansorySupplyHub.Dto;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IOrderHeaderService
    {
        Task<ResponseModel<OrderHeaderDto>> CreateOrderHeader(CreateOrderHeaderDto request);
        Task<ResponseModel<OrderHeaderDto>> EditOrderHeader(UpdateOrderHeaderDto request, int id);
        Task<ResponseModel<bool>> DeleteOrderHeader(int id);
        Task<ResponseModel<List<OrderHeaderDto>>> GetAllOrderHeaders();
        Task<ResponseModel<OrderHeaderDto>> GetOrderHeaderDetails(int id);
    }
}
