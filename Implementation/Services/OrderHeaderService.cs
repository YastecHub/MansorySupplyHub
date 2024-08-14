using MansorySupplyHub.Data;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace MansorySupplyHub.Implementation.Services
{
    public class OrderHeaderService : IOrderHeaderService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<OrderHeaderService> _logger;

        public OrderHeaderService(ApplicationDbContext dbContext, ILogger<OrderHeaderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseModel<OrderHeaderDto>> CreateOrderHeader(CreateOrderHeaderDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new order header.");

                var orderHeader = new OrderHeader
                {
                    CreatedByUserId = request.CreatedByUserId,
                    OrderDate = request.OrderDate,
                    ShippingDate = request.ShippingDate,
                    FinalOrderTotal = request.FinalOrderTotal,
                    OrderStatus = request.OrderStatus,
                    PaymentDate = request.PaymentDate,
                    TransactionId = request.TransactionId ?? Guid.NewGuid().ToString(),
                    PhoneNumber = request.PhoneNumber,
                    StreetAddress = request.StreetAddress,
                    City = request.City,
                    State = request.State,
                    PostalCode = request.PostalCode,
                    FullName = request.FullName,
                    Email = request.Email
                };

                _dbContext.OrderHeaders.Add(orderHeader);
                await _dbContext.SaveChangesAsync();

                var orderHeaderDto = new OrderHeaderDto
                {
                    Id = orderHeader.Id,
                    CreatedByUserId = orderHeader.CreatedByUserId,
                    OrderDate = orderHeader.OrderDate,
                    ShippingDate = orderHeader.ShippingDate,
                    FinalOrderTotal = orderHeader.FinalOrderTotal,
                    OrderStatus = orderHeader.OrderStatus,
                    PaymentDate = orderHeader.PaymentDate,
                    TransactionId = orderHeader.TransactionId,
                    PhoneNumber = orderHeader.PhoneNumber,
                    StreetAddress = orderHeader.StreetAddress,
                    City = orderHeader.City,
                    State = orderHeader.State,
                    PostalCode = orderHeader.PostalCode,
                    FullName = orderHeader.FullName,
                    Email = orderHeader.Email
                };

                _logger.LogInformation("Order header created successfully.");

                return new ResponseModel<OrderHeaderDto>
                {
                    Success = true,
                    Data = orderHeaderDto,
                    Message = "Order header created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order header.");

                return new ResponseModel<OrderHeaderDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the order header."
                };
            }
        }

        public async Task<ResponseModel<OrderHeaderDto>> EditOrderHeader(UpdateOrderHeaderDto request, int id)
        {
            try
            {
                _logger.LogInformation("Editing order header: {OrderHeaderId}", id);

                var orderHeader = await _dbContext.OrderHeaders.FindAsync(id);
                if (orderHeader == null)
                {
                    _logger.LogWarning("Order header not found: {OrderHeaderId}", id);
                    return new ResponseModel<OrderHeaderDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Order header not found."
                    };
                }

                orderHeader.CreatedByUserId = request.CreatedByUserId;
                orderHeader.OrderDate = request.OrderDate;
                orderHeader.ShippingDate = request.ShippingDate;
                orderHeader.FinalOrderTotal = request.FinalOrderTotal;
                orderHeader.OrderStatus = request.OrderStatus;
                orderHeader.PaymentDate = request.PaymentDate;
                orderHeader.TransactionId = request.TransactionId;
                orderHeader.PhoneNumber = request.PhoneNumber;
                orderHeader.StreetAddress = request.StreetAddress;
                orderHeader.City = request.City;
                orderHeader.State = request.State;
                orderHeader.PostalCode = request.PostalCode;
                orderHeader.FullName = request.FullName;
                orderHeader.Email = request.Email;

                _dbContext.OrderHeaders.Update(orderHeader);
                await _dbContext.SaveChangesAsync();

                var orderHeaderDto = new OrderHeaderDto
                {
                    Id = orderHeader.Id,
                    CreatedByUserId = orderHeader.CreatedByUserId,
                    OrderDate = orderHeader.OrderDate,
                    ShippingDate = orderHeader.ShippingDate,
                    FinalOrderTotal = orderHeader.FinalOrderTotal,
                    OrderStatus = orderHeader.OrderStatus,
                    PaymentDate = orderHeader.PaymentDate,
                    TransactionId = orderHeader.TransactionId,
                    PhoneNumber = orderHeader.PhoneNumber,
                    StreetAddress = orderHeader.StreetAddress,
                    City = orderHeader.City,
                    State = orderHeader.State,
                    PostalCode = orderHeader.PostalCode,
                    FullName = orderHeader.FullName,
                    Email = orderHeader.Email
                };

                _logger.LogInformation("Order header updated successfully: {OrderHeaderId}", id);

                return new ResponseModel<OrderHeaderDto>
                {
                    Success = true,
                    Data = orderHeaderDto,
                    Message = "Order header updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order header: {OrderHeaderId}", id);

                return new ResponseModel<OrderHeaderDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the order header."
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteOrderHeader(int id)
        {
            try
            {
                _logger.LogInformation("Deleting order header: {OrderHeaderId}", id);

                var orderHeader = await _dbContext.OrderHeaders.FindAsync(id);
                if (orderHeader == null)
                {
                    _logger.LogWarning("Order header not found: {OrderHeaderId}", id);
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Order header not found."
                    };
                }

                _dbContext.OrderHeaders.Remove(orderHeader);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Order header deleted successfully: {OrderHeaderId}", id);

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Order header deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the order header: {OrderHeaderId}", id);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while deleting the order header."
                };
            }
        }

        public async Task<ResponseModel<List<OrderHeaderDto>>> GetAllOrderHeaders()
        {
            try
            {
                _logger.LogInformation("Retrieving all order headers.");

                var orderHeaders = await _dbContext.OrderHeaders.ToListAsync();

                var orderHeaderDtos = orderHeaders.Select(header => new OrderHeaderDto
                {
                    Id = header.Id,
                    CreatedByUserId = header.CreatedByUserId,
                    OrderDate = header.OrderDate,
                    ShippingDate = header.ShippingDate,
                    FinalOrderTotal = header.FinalOrderTotal,
                    OrderStatus = header.OrderStatus,
                    PaymentDate = header.PaymentDate,
                    TransactionId = header.TransactionId,
                    PhoneNumber = header.PhoneNumber,
                    StreetAddress = header.StreetAddress,
                    City = header.City,
                    State = header.State,
                    PostalCode = header.PostalCode,
                    FullName = header.FullName,
                    Email = header.Email
                }).ToList();

                _logger.LogInformation("Order headers retrieved successfully.");

                return new ResponseModel<List<OrderHeaderDto>>
                {
                    Data = orderHeaderDtos,
                    Success = true,
                    Message = "Order headers retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order headers.");

                return new ResponseModel<List<OrderHeaderDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving order headers."
                };
            }
        }

        public async Task<ResponseModel<OrderHeaderDto>> GetOrderHeaderDetails(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving order header details for order header: {OrderHeaderId}", id);

                var orderHeader = await _dbContext.OrderHeaders
                    .Include(header => header.OrderDetails) 
                    .FirstOrDefaultAsync(header => header.Id == id);

                if (orderHeader == null)
                {
                    _logger.LogWarning("Order header not found: {OrderHeaderId}", id);
                    return new ResponseModel<OrderHeaderDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Order header not found."
                    };
                }
                var orderHeaderDto = new OrderHeaderDto
                {
                    Id = orderHeader.Id,
                    CreatedByUserId = orderHeader.CreatedByUserId,
                    OrderDate = orderHeader.OrderDate,
                    ShippingDate = orderHeader.ShippingDate,
                    FinalOrderTotal = orderHeader.FinalOrderTotal,
                    OrderStatus = orderHeader.OrderStatus,
                    PaymentDate = orderHeader.PaymentDate,
                    TransactionId = orderHeader.TransactionId,
                    PhoneNumber = orderHeader.PhoneNumber,
                    StreetAddress = orderHeader.StreetAddress,
                    City = orderHeader.City,
                    State = orderHeader.State,
                    PostalCode = orderHeader.PostalCode,
                    FullName = orderHeader.FullName,
                    Email = orderHeader.Email
                };

                _logger.LogInformation("Order header details retrieved successfully for order header: {OrderHeaderId}", id);

                return new ResponseModel<OrderHeaderDto>
                {
                    Data = orderHeaderDto,
                    Success = true,
                    Message = "Order header details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order header details: {OrderHeaderId}", id);

                return new ResponseModel<OrderHeaderDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving order header details."
                };
            }
        }
    }
}
