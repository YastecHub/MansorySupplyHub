using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<OrderDetailService> _logger;

        public OrderDetailService(ApplicationDbContext dbContext, ILogger<OrderDetailService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseModel<OrderDetailDto>> CreateOrderDetail(CreateOrderDetailDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new order detail.");

                var orderDetail = new OrderDetail
                {
                    OrderHeaderId = request.OrderHeaderId,
                    ProductId = request.ProductId,
                    Sqft = request.Sqft,
                    PricePerSqFt = request.PricePerSqFt,
                   // CreatedDate = DateTime.Now
                    
                };

                _dbContext.OrderDetails.Add(orderDetail);
                await _dbContext.SaveChangesAsync();

                var orderDetailDto = new OrderDetailDto
                {
                    Id = orderDetail.Id,
                    OrderHeaderId = orderDetail.OrderHeaderId,
                    ProductId = orderDetail.ProductId,
                    Sqft = orderDetail.Sqft,
                    PricePerSqFt = orderDetail.PricePerSqFt
                };

                _logger.LogInformation("Order detail created successfully.");

                return new ResponseModel<OrderDetailDto>
                {
                    Success = true,
                    Data = orderDetailDto,
                    Message = "Order detail created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order detail.");

                return new ResponseModel<OrderDetailDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the order detail."
                };
            }
        }

        public async Task<ResponseModel<OrderDetailDto>> EditOrderDetail(UpdateOrderDetailDto request)
        {
            try
            {
                _logger.LogInformation("Editing order detail: {OrderDetailId}", request.OrderHeaderId);

                var orderDetail = await _dbContext.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderHeaderId == request.OrderHeaderId && od.ProductId == request.ProductId);

                if (orderDetail == null)
                {
                    _logger.LogWarning("Order detail not found: {OrderDetailId}", request.OrderHeaderId);
                    return new ResponseModel<OrderDetailDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Order detail not found."
                    };
                }

                orderDetail.Sqft = request.Sqft;
                orderDetail.PricePerSqFt = request.PricePerSqFt;
               // orderDetail.UpdatedDate = DateTime.Now;

                _dbContext.OrderDetails.Update(orderDetail);
                await _dbContext.SaveChangesAsync();

                var orderDetailDto = new OrderDetailDto
                {
                    Id = orderDetail.Id,
                    OrderHeaderId = orderDetail.OrderHeaderId,
                    ProductId = orderDetail.ProductId,
                    Sqft = orderDetail.Sqft,
                    PricePerSqFt = orderDetail.PricePerSqFt
                };

                _logger.LogInformation("Order detail updated successfully: {OrderDetailId}", request.OrderHeaderId);

                return new ResponseModel<OrderDetailDto>
                {
                    Success = true,
                    Data = orderDetailDto,
                    Message = "Order detail updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order detail: {OrderDetailId}", request.OrderHeaderId);

                return new ResponseModel<OrderDetailDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the order detail."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteOrderDetail(int id)
        {
            try
            {
                _logger.LogInformation("Deleting order detail: {OrderDetailId}", id);

                var orderDetail = await _dbContext.OrderDetails.FindAsync(id);
                if (orderDetail == null)
                {
                    _logger.LogWarning("Order detail not found: {OrderDetailId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Order detail not found."
                    };
                }

                _dbContext.OrderDetails.Remove(orderDetail);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Order detail deleted successfully: {OrderDetailId}", id);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Order detail deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the order detail: {OrderDetailId}", id);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the order detail."
                };
            }
        }

        public async Task<ResponseModel<List<OrderDetailDto>>> GetOrderDetailsByHeaderId(int orderHeaderId)
        {
            try
            {
                _logger.LogInformation("Retrieving order details for header: {OrderHeaderId}", orderHeaderId);

                var orderDetails = await _dbContext.OrderDetails
                    .Include(d => d.Product) 
                    .Where(d => d.OrderHeaderId == orderHeaderId)
                    .ToListAsync();

                var orderDetailDtos = orderDetails.Select(detail => new OrderDetailDto
                {
                    Id = detail.Id,
                    OrderHeaderId = detail.OrderHeaderId,
                    ProductId = detail.ProductId,
                    Sqft = detail.Sqft,
                    PricePerSqFt = detail.PricePerSqFt,
                    Product = detail.Product != null ? new ProductDto
                    {
                        Id = detail.Product.Id,
                        Name = detail.Product.Name,
                        Price = detail.Product.Price
                    } : null
                }).ToList();

                _logger.LogInformation("Order details retrieved successfully for header: {OrderHeaderId}", orderHeaderId);

                return new ResponseModel<List<OrderDetailDto>>
                {
                    Data = orderDetailDtos,
                    Success = true,
                    Message = "Order details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details for header: {OrderHeaderId}", orderHeaderId);

                return new ResponseModel<List<OrderDetailDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving order details."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteOrderDetailsByHeaderId(int orderHeaderId)
        {
            try
            {
                _logger.LogInformation("Deleting order details for header: {OrderHeaderId}", orderHeaderId);

                var orderDetails = await _dbContext.OrderDetails
                    .Where(d => d.OrderHeaderId == orderHeaderId)
                    .ToListAsync();

                if (orderDetails == null || !orderDetails.Any())
                {
                    _logger.LogWarning("No order details found for header: {OrderHeaderId}", orderHeaderId);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "No order details found for the specified header."
                    };
                }

                _dbContext.OrderDetails.RemoveRange(orderDetails);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Order details deleted successfully for header: {OrderHeaderId}", orderHeaderId);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Order details deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting order details for header: {OrderHeaderId}", orderHeaderId);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting order details."
                };
            }
        }
    }
}
