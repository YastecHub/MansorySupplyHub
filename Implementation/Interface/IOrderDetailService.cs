using MansorySupplyHub.Dto;
using MansorySupplyHub.Models;

namespace MansorySupplyHub.Implementation.Interface
{
    public interface IOrderDetailService
    {
        Task<ResponseModel<OrderDetailDto>> CreateOrderDetail(CreateOrderDetailDto request);
        Task<ResponseModel<OrderDetailDto>> EditOrderDetail(UpdateOrderDetailDto request);
        Task<ResponseModel<bool>> DeleteOrderDetail(int id);
        Task<ResponseModel<List<OrderDetailDto>>> GetOrderDetailsByHeaderId(int orderHeaderId);
        Task<ResponseModel<bool>> DeleteOrderDetailsByHeaderId(int orderHeaderId);
    }
}
