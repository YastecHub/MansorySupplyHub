using Braintree;
using MansorySupplyHub.BrainTree;
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
        private readonly IBrainTreeGate _brainTreeGat;

        public OrderHeaderService(ApplicationDbContext dbContext, ILogger<OrderHeaderService> logger, IBrainTreeGate brainTreeGat)
        {
            _dbContext = dbContext;
            _logger = logger;
            _brainTreeGat = brainTreeGat;
        }


        public async Task<ResponseModel<IEnumerable<OrderHeaderDto>>> FilterOrderHeadersAsync(IEnumerable<OrderHeaderDto> orders,
                                                                                          string searchName = null,
                                                                                          string searchEmail = null,
                                                                                          string searchPhone = null,
                                                                                          string status = null)
        {
            var filteredOrders = orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                filteredOrders = filteredOrders.Where(u => u.FullName.Contains(searchName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                filteredOrders = filteredOrders.Where(u => u.Email.Contains(searchEmail, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                filteredOrders = filteredOrders.Where(u => u.PhoneNumber.Contains(searchPhone, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(status) && status != "--Order Status--")
            {
                filteredOrders = filteredOrders.Where(u => u.OrderStatus.Equals(status, StringComparison.OrdinalIgnoreCase));
            }


            return new ResponseModel<IEnumerable<OrderHeaderDto>>
            {
                Success = true,
                Data = await Task.FromResult(filteredOrders.ToList())
            };
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
                    OrderStatus = request.OrderStatus ?? "Pending",
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
                    OrderStatus = orderHeader.OrderStatus ?? "Pending",
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


        public async Task<ResponseModel<bool>> StartProcessingOrder(int orderId)
        {
            try
            {
                _logger.LogInformation("Starting processing for order header: {OrderHeaderId}", orderId);

                var response = await GetOrderHeaderDetails(orderId);
                if (!response.Success || response.Data == null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Order header not found."
                    };
                }

                var orderHeader = response.Data;
                orderHeader.OrderStatus = WC.StatusInProcess;

                var updateResponse = await EditOrderHeader(new UpdateOrderHeaderDto
                {
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
                }, orderId);

                if (!updateResponse.Success)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Failed to update order status to In Process."
                    };
                }

                _logger.LogInformation("Order status set to In Process for order header: {OrderHeaderId}", orderId);

                return new ResponseModel<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Order is In Process."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting processing for order header: {OrderHeaderId}", orderId);

                return new ResponseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "An error occurred while processing the order."
                };
            }
        }


        public async Task<ResponseModel<bool>> ShipOrder(int orderId)
        {
            try
            {
                _logger.LogInformation("Shipping order: {OrderHeaderId}", orderId);

                var response = await GetOrderHeaderDetails(orderId);
                if (!response.Success || response.Data == null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Order header not found."
                    };
                }

                var orderHeader = response.Data;
                orderHeader.OrderStatus = WC.StatusShipped;
                orderHeader.ShippingDate = DateTime.Now;

                var updateResponse = await EditOrderHeader(new UpdateOrderHeaderDto
                {
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
                }, orderId);

                if (!updateResponse.Success)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Failed to update order status to Shipped."
                    };
                }

                _logger.LogInformation("Order shipped successfully: {OrderHeaderId}", orderId);

                return new ResponseModel<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Order shipped successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while shipping the order: {OrderHeaderId}", orderId);

                return new ResponseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "An error occurred while processing the shipment."
                };
            }
        }

        public async Task<ResponseModel<bool>> CancelOrder(int orderId)
        {
            try
            {
                _logger.LogInformation("Cancelling order: {OrderHeaderId}", orderId);

                var response = await GetOrderHeaderDetails(orderId);
                if (!response.Success || response.Data == null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Order not found."
                    };
                }

                var orderHeader = response.Data;

                // Uncomment and configure Braintree transaction cancellation when ready
                // var gateway = _brainTreeGate.GetGateway();
                // Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);
                // if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
                // {
                //     // Void the transaction if it hasn't been settled yet
                //     Result<Transaction> resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
                // }
                // else
                // {
                //     // Refund the transaction if it has already been settled
                //     Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
                // }

                orderHeader.OrderStatus = WC.StatusRefunded;

                var updateResponse = await EditOrderHeader(new UpdateOrderHeaderDto
                {
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
                }, orderId);

                if (!updateResponse.Success)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Failed to update order status to Refunded."
                    };
                }

                _logger.LogInformation("Order cancelled and status updated to refunded: {OrderHeaderId}", orderId);

                return new ResponseModel<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Order cancelled and refunded successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cancelling the order: {OrderHeaderId}", orderId);

                return new ResponseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "An error occurred while processing the cancellation."
                };
            }
        }


        public async Task<ResponseModel<bool>> UpdateOrderDetails(OrderVM orderVM)
        {
            var response = await GetOrderHeaderDetails(orderVM.OrderHeader.Id);
            if (!response.Success || response.Data == null)
            {
                return new ResponseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Order not found."
                };
            }

            var orderHeader = response.Data;

            // Update order details based on the orderVM
            orderHeader.FullName = orderVM.OrderHeader.FullName;
            orderHeader.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeader.City = orderVM.OrderHeader.City;
            orderHeader.State = orderVM.OrderHeader.State;
            orderHeader.PostalCode = orderVM.OrderHeader.PostalCode;
            orderHeader.Email = orderVM.OrderHeader.Email;

            // Map to UpdateOrderHeaderDto
            var updateOrderHeaderDto = MapToUpdateOrderHeaderDto(orderHeader);

            // Call EditOrderHeader to save the updated details
            var updateResponse = await EditOrderHeader(updateOrderHeaderDto, orderHeader.Id);
            if (!updateResponse.Success)
            {
                return new ResponseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Failed to update order details."
                };
            }

            return new ResponseModel<bool>
            {
                Success = true,
                Data = true,
                Message = "Order details updated successfully."
            };
        }

        private UpdateOrderHeaderDto MapToUpdateOrderHeaderDto(OrderHeaderDto orderHeaderDto)
        {
            return new UpdateOrderHeaderDto
            {
                CreatedByUserId = orderHeaderDto.CreatedByUserId,
                OrderDate = orderHeaderDto.OrderDate,
                ShippingDate = orderHeaderDto.ShippingDate,
                FinalOrderTotal = orderHeaderDto.FinalOrderTotal,
                OrderStatus = orderHeaderDto.OrderStatus,
                PaymentDate = orderHeaderDto.PaymentDate,
                TransactionId = orderHeaderDto.TransactionId,
                PhoneNumber = orderHeaderDto.PhoneNumber,
                StreetAddress = orderHeaderDto.StreetAddress,
                City = orderHeaderDto.City,
                State = orderHeaderDto.State,
                PostalCode = orderHeaderDto.PostalCode,
                FullName = orderHeaderDto.FullName,
                Email = orderHeaderDto.Email
            };
        }


    }
}
