using Braintree;
using MansorySupplyHub.BrainTree;
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
        private readonly ILogger<CartService> _logger;
        private readonly IEmailService _emailService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IInquiryHeaderService _inqHService;
        private readonly IInquiryDetailService _inqDService;
        private readonly IBrainTreeGate _brainTreeGate;

        public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<CartService> logger, IEmailService emailService,IOrderHeaderService orderHeaderService,IOrderDetailService orderDetailService, IInquiryHeaderService inqHService, IInquiryDetailService inqDService, IBrainTreeGate brainTreeGate)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _emailService = emailService;
            _orderHeaderService = orderHeaderService;
            _orderDetailService = orderDetailService;
            _inqHService = inqHService;
            _inqDService = inqDService;
            _brainTreeGate = brainTreeGate;
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public async Task<ResponseModel<List<ProductDto>>> GetProductsInCart(List<int> productIds)
        {
            try
            {
                _logger.LogInformation("Retrieving products in cart for product IDs: {ProductIds}", productIds);

                var products = await _dbContext.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Description = p.Description,
                }).ToList();

                _logger.LogInformation("Products retrieved successfully.");

                return new ResponseModel<List<ProductDto>>
                {
                    Data = productDtos,
                    Success = true,
                    Message = "Products retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products in the cart.");

                return new ResponseModel<List<ProductDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving products in the cart."
                };
            }
            finally
            {
                _logger.LogDebug("GetProductsInCart method execution completed.");
            }
        }

        public async Task<ResponseModel<ProductUserDto>> GetUserCartDetails(string userId, List<int> productIds)
        {
            try
            {
                _logger.LogInformation("Retrieving user cart details for user ID: {UserId}", userId);

                var user = await _dbContext.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
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
                    _logger.LogWarning("Failed to retrieve products for user ID: {UserId}", userId);
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

                _logger.LogInformation("User and cart details retrieved successfully for user ID: {UserId}", userId);

                return new ResponseModel<ProductUserDto>
                {
                    Data = userCartDetails,
                    Success = true,
                    Message = "User and cart details retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user cart details for user ID: {UserId}", userId);

                return new ResponseModel<ProductUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while retrieving user cart details."
                };
            }
            finally
            {
                _logger.LogDebug("GetUserCartDetails method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> ClearCart()
        {
            try
            {
                _logger.LogInformation("Clearing the cart.");

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("HttpContext is null.");
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "HttpContext is null."
                    };
                }

                httpContext.Session.Set(WC.SessionCart, new List<ShoppingCart>());

                _logger.LogInformation("Cart cleared successfully.");

                return new ResponseModel<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Cart cleared successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while clearing the cart.");

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while clearing the cart."
                };
            }
            finally
            {
                _logger.LogDebug("ClearCart method execution completed.");
            }
        }

        public async Task<ResponseModel<bool>> RemoveProductFromCart(int productId, List<ShoppingCart> shoppingCartList)
        {
            try
            {
                _logger.LogInformation("Removing product from cart: {ProductId}", productId);

                var productToRemove = shoppingCartList.FirstOrDefault(c => c.ProductId == productId);
                if (productToRemove != null)
                {
                    shoppingCartList.Remove(productToRemove);
                    _logger.LogInformation("Product removed from cart: {ProductId}", productId);

                    return new ResponseModel<bool>
                    {
                        Data = true,
                        Success = true,
                        Message = "Product removed from cart."
                    };
                }

                _logger.LogWarning("Product not found in cart: {ProductId}", productId);
                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Product not found in cart."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the product from the cart: {ProductId}", productId);

                return new ResponseModel<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "An error occurred while removing the product from the cart."
                };
            }
            finally
            {
                _logger.LogDebug("RemoveProductFromCart method execution completed.");
            }
        }

        public async Task<ResponseModel<List<ProductDto>>> GetCartProductsWithQuantities(HttpContext httpContext)
        {
            // Retrieve the session cart data
            var shoppingCartSession = httpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();

            // Get list of product IDs
            var productIds = shoppingCartList.Select(i => i.ProductId).ToList();

            // Fetch products
            var productResult = await GetProductsInCart(productIds);
            if (!productResult.Success)
            {
                return new ResponseModel<List<ProductDto>>
                {
                    Success = false,
                    Message = "Failed to load cart items."
                };
            }

            // Assign quantities from cart to products
            var products = productResult.Data;
            foreach (var cartItem in shoppingCartList)
            {
                var product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product != null)
                {
                    product.TempSqft = cartItem.Sqft;
                }
            }

            return new ResponseModel<List<ProductDto>>
            {
                Success = true,
                Data = products
            };
        }

        public async Task UpdateSessionCart(IEnumerable<Product> products, HttpContext httpContext)
        {
            // Prepare the list of shopping cart items from the product list
            var shoppingCartList = products.Select(prod => new ShoppingCart
            {
                ProductId = prod.Id,
                Sqft = prod.TempSqft
            }).ToList();

            // Update the session cart
            httpContext.Session.Set(WC.SessionCart, shoppingCartList);
        }

        public async Task<ResponseModel<ProductUserDto>> GetCartSummaryAsync(string userId)
        {
            var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();

            if (!shoppingCartList.Any())
            {
                return new ResponseModel<ProductUserDto>
                {
                    Success = false,
                    Message = "Your cart is empty."
                };
            }

            var productIds = shoppingCartList.Select(i => i.ProductId).ToList();
            var result = await GetUserCartDetails(userId, productIds);

            if (!result.Success)
            {
                return new ResponseModel<ProductUserDto>
                {
                    Success = false,
                    Message = "Failed to load cart summary."
                };
            }

            var productUserDto = result.Data;

            // Map TempSqft and other properties from session to the ProductUserDto
            foreach (var cartItem in shoppingCartList)
            {
                var product = productUserDto.ProductList.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product != null)
                {
                    product.TempSqft = cartItem.Sqft;
                    product.TotalPrice = product.TempSqft * product.Price; // Assuming you have a TotalPrice property
                }
            }

            return new ResponseModel<ProductUserDto> 
            { 
                Success = true, 
                Data = productUserDto 
            };
        }

        public async Task<ResponseModel<OrderHeaderDto>> CreateOrderAsync(ProductUserDto productUserDto, string userId, IEnumerable<ShoppingCart> shoppingCartList, string nonceFromTheClient)
        {
            // Create the order header request
            var orderHeaderRequest = new CreateOrderHeaderDto
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                FullName = productUserDto.ApplicationUser.FullName,
                PhoneNumber = productUserDto.ApplicationUser.PhoneNumber,
                Email = productUserDto.ApplicationUser.Email,
                City = productUserDto.ApplicationUser.City,
                State = productUserDto.ApplicationUser.State,
                CreatedByUserId = userId,
                PostalCode = productUserDto.ApplicationUser.PostalCode,
                StreetAddress = productUserDto.ApplicationUser.StreetAddress,
                OrderStatus = WC.StatusPending,
                FinalOrderTotal = productUserDto.ProductList.Sum(x => x.TempSqft * x.Price)
            };

            // Call the existing method to create the order header
            var orderHeaderResponse = await _orderHeaderService.CreateOrderHeader(orderHeaderRequest);
            if (!orderHeaderResponse.Success) return new ResponseModel<OrderHeaderDto> { Success = false, Message = orderHeaderResponse.Message };

            var orderHeaderId = orderHeaderResponse.Data.Id;

            foreach (var cartItem in shoppingCartList)
            {
                var orderDetail = new CreateOrderDetailDto
                {
                    OrderHeaderId = orderHeaderId,
                    ProductId = cartItem.ProductId,
                    Sqft = cartItem.TempSqft,
                    PricePerSqFt = productUserDto.ProductList.FirstOrDefault(p => p.Id == cartItem.ProductId)?.Price ?? 0
                };

                var orderDetailResponse = await _orderDetailService.CreateOrderDetail(orderDetail);
                if (!orderDetailResponse.Success)
                {
                    return new ResponseModel<OrderHeaderDto> 
                    { 
                        Success = false, 
                        Message = "Failed to create order detail." 
                    };
                }
            }

            // Process payment
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(orderHeaderRequest.FinalOrderTotal),
                PaymentMethodNonce = nonceFromTheClient ?? "",
                OrderId = orderHeaderResponse.Data.Id.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = _brainTreeGate.GetGateway();
            var result = gateway.Transaction.Sale(request);

            if (result == null || result.Target == null)
            {
                // Update order status if payment fails
                orderHeaderResponse.Data.OrderStatus = WC.StatusCancelled;
                return new ResponseModel<OrderHeaderDto> 
                { 
                    Success = false, 
                    Message = "Payment transaction failed." 
                };
            }

            // Return the successful order header response
            return orderHeaderResponse;
        }

        public async Task<ResponseModel<InquiryHeaderDto>> CreateInquiryAsync(ProductUserDto productUserDto, string userId, IEnumerable<ShoppingCart> shoppingCartList)
        {
            var inquiryHeader = new CreateInquiryHeaderDto
            {
                ApplicationUserId = userId,
                InquiryDate = DateTime.Now,
                FullName = productUserDto.ApplicationUser.FullName,
                PhoneNumber = productUserDto.ApplicationUser.PhoneNumber,
                Email = productUserDto.ApplicationUser.Email
            };

            var inquiryHeaderResponse = await _inqHService.CreateInquiryHeader(inquiryHeader);
            if (!inquiryHeaderResponse.Success) return inquiryHeaderResponse;

            var inquiryHeaderId = inquiryHeaderResponse.Data.Id;

            foreach (var cartItem in shoppingCartList)
            {
                var inquiryDetail = new CreateInquiryDetailDto
                {
                    InquiryHeaderId = inquiryHeaderId,
                    ProductId = cartItem.ProductId,
                    Sqft = cartItem.Sqft
                };

                var inquiryDetailResponse = await _inqDService.CreateInquiryDetail(inquiryDetail);
                if (!inquiryDetailResponse.Success) return new ResponseModel<InquiryHeaderDto>
                {
                    Success = false, 
                    Message = "Failed to create inquiry detail."
                };
            }

            return inquiryHeaderResponse;
        }
    }
}
