using Braintree;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IOrderHeaderService
    {
        Task<ResponseModel<IEnumerable<OrderHeaderDto>>> FilterOrderHeadersAsync(IEnumerable<OrderHeaderDto> orders,
                                                                                          string searchName = null,
                                                                                          string searchEmail = null,
                                                                                          string searchPhone = null,
                                                                                          string status = null);

        Task<ResponseModel<OrderHeaderDto>> CreateOrderHeader(CreateOrderHeaderDto request);
        Task<ResponseModel<OrderHeaderDto>> EditOrderHeader(UpdateOrderHeaderDto request, int id);
        Task<ResponseModel<bool>> DeleteOrderHeader(int id);
        Task<ResponseModel<List<OrderHeaderDto>>> GetAllOrderHeaders();
        Task<ResponseModel<OrderHeaderDto>> GetOrderHeaderDetails(int id);
        Task<ResponseModel<bool>> StartProcessingOrder(int orderId);
        Task<ResponseModel<bool>> ShipOrder(int orderId);
        Task<ResponseModel<bool>> CancelOrder(int orderId);
        Task<ResponseModel<bool>> UpdateOrderDetails(OrderVM orderVM);
    }
}
