using AspNetCoreHero.ToastNotification.Abstractions;
using Braintree;
using MansorySupplyHub.BrainTree;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Implementation.Services;
using MansorySupplyHub.Models;
using MansorySupplyHub.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MansorySupplyHub.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IInquiryHeaderService _inqHService;
        private readonly IInquiryDetailService _inqDService;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IBrainTreeGate _brainTreeGate;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ICartService cartService, IInquiryHeaderService inqHService, IInquiryDetailService inqDService, INotyfService notyf, IEmailService emailService, IOrderHeaderService orderHeaderService, IOrderDetailService orderDetailService, IBrainTreeGate brainTreeGate)
        {
            _cartService = cartService;
            _inqHService = inqHService;
            _inqDService = inqDService;

            _notyf = notyf;
            _emailService = emailService;
            _orderHeaderService = orderHeaderService;
            _orderDetailService = orderDetailService;
            _brainTreeGate = brainTreeGate;
        }

        [HttpGet("cart-items")]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();
            var productIds = shoppingCartList.Select(i => i.ProductId).ToList();

            var result = await _cartService.GetProductsInCart(productIds);
            if (result.Success)
            {
                var products = result.Data;

                foreach (var cartItem in shoppingCartList)
                {
                    var product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                    if (product != null)
                    {
                        product.TempSqft = cartItem.Sqft;
                    }
                }

                return View(products);
            }

            _notyf.Error("Failed to load cart items.");
            return View(new List<ProductDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Sqft = prod.TempSqft });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Summary));
        }

        [HttpGet("cart-summary")]
        public async Task<IActionResult> Summary()
        {
            var gateway = _brainTreeGate.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _notyf.Error("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();

            if (!shoppingCartList.Any())
            {
                _notyf.Warning("Your cart is empty.");
                return RedirectToAction("Index");
            }

            var productIds = shoppingCartList.Select(i => i.ProductId).ToList();
            var result = await _cartService.GetUserCartDetails(userId, productIds);

            if (!result.Success)
            {
                _notyf.Error("Failed to load cart summary. Please try again.");
                return RedirectToAction("Index");
            }

            var productUserDto = result.Data;

            // Map TempSqft and other properties from session to the ProductUserDto
            foreach (var cartItem in shoppingCartList)
            {
                var product = productUserDto.ProductList.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product != null)
                {
                    product.TempSqft = cartItem.Sqft;
                    // Calculate total price based on TempSqft and Price
                    product.Price = product.Price; // Ensure price is not null
                    product.TotalPrice = product.TempSqft * product.Price; // Assuming you have a TotalPrice property
                }
            }

            return View(productUserDto);
        }

        [HttpPost("cart-summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost(ProductUserDto productUserDto, IFormCollection collection)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _notyf.Error("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();

            if (!shoppingCartList.Any())
            {
                _notyf.Warning("Your cart is empty.");
                return RedirectToAction(nameof(Index));
            }
            //var orderTotal = 0.0;
            if (User.IsInRole(WC.AdminRole))
            {
                //foreach (var cartItem in shoppingCartList)
                //{
                //    var product = productUserDto.ProductList.FirstOrDefault(p => p.Id == cartItem.ProductId);
                //    if (product != null)
                //    {
                //        // Assume product.TotalPrice was calculated in the Summary view
                //        orderTotal += product.TotalPrice;
                //    }
                //}
                // Create an Order for Admin
                var orderHeader = new CreateOrderHeaderDto
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
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempSqft * x.Price),
                };

                var orderHeaderResponse = await _orderHeaderService.CreateOrderHeader(orderHeader);
                if (!orderHeaderResponse.Success)
                {
                    _notyf.Error("Failed to create order.");
                    return RedirectToAction(nameof(Summary));
                }

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
                        _notyf.Error("Failed to create order detail.");
                        return RedirectToAction(nameof(Summary));
                    }
                }

                string nonceFromTheClient = collection["payment_method_nonce"];
                _notyf.Information($"Nonce received: {nonceFromTheClient}");

                // Log the transaction request details
                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient ?? "",
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };


                _notyf.Information($"Transaction Request - Amount: {request.Amount}, OrderId: {request.OrderId}");

                var gateway = _brainTreeGate.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result == null)
                {
                    _notyf.Error("Payment transaction failed: No response from the payment gateway.");
                    orderHeader.OrderStatus = WC.StatusCancelled;
                    return RedirectToAction(nameof(Summary));
                }

                if (result.Target == null)
                {
                    _notyf.Error("Payment transaction failed: Target was null.");
                    orderHeader.OrderStatus = WC.StatusCancelled;
                    return RedirectToAction(nameof(Summary));
                }

                return RedirectToAction(nameof(Summary));

            }
            else
            {
                // Create an Inquiry for regular users
                var inquiryHeader = new CreateInquiryHeaderDto
                {
                    ApplicationUserId = userId,
                    InquiryDate = DateTime.Now,
                    FullName = productUserDto.ApplicationUser.FullName,
                    PhoneNumber = productUserDto.ApplicationUser.PhoneNumber,
                    Email = productUserDto.ApplicationUser.Email
                };

                var inquiryHeaderResponse = await _inqHService.CreateInquiryHeader(inquiryHeader);
                if (!inquiryHeaderResponse.Success)
                {
                    _notyf.Error("Failed to create inquiry header.");
                    return RedirectToAction(nameof(Summary));
                }

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
                    if (!inquiryDetailResponse.Success)
                    {
                        _notyf.Error("Failed to create inquiry detail.");
                        return RedirectToAction(nameof(Summary));
                    }
                }

                // Send email confirmation
                var profile = new Profile
                {
                    FirstName = productUserDto.ApplicationUser.FullName,
                    Email = productUserDto.ApplicationUser.Email
                };

                var emailResponse = await _emailService.SendNotificationToUserAsync(profile);
                if (emailResponse.Success)
                {
                    _notyf.Success("Inquiry submitted successfully. A confirmation email has been sent.");
                }
                else
                {
                    _notyf.Warning("Inquiry submitted, but the confirmation email could not be sent.");
                }

                HttpContext.Session.Clear();
                return RedirectToAction(nameof(InquiryConfirmation), new { id = inquiryHeaderId, isOrder = false });
            }
        }


        [HttpGet("cart-confirmation")]
        public async Task<IActionResult> InquiryConfirmation(int id, bool isOrder)
        {
            if (id > 0)
            {
                if (isOrder)
                {
                    var orderResponse = await _orderHeaderService.GetOrderHeaderDetails(id);
                    if (orderResponse.Success)
                    {
                        ViewBag.IsOrder = true;
                        ViewBag.Id = orderResponse.Data.Id;
                        return View();
                    }
                }
                else
                {
                    var inquiryResponse = await _inqHService.GetInquiryHeaderDetails(id);
                    if (inquiryResponse.Success)
                    {
                        ViewBag.IsOrder = false;
                        ViewBag.Id = inquiryResponse.Data.Id;
                        return View();
                    }
                }

                _notyf.Error("Failed to load order or inquiry confirmation.");
                return RedirectToAction("Index");
            }

            _notyf.Error("Invalid confirmation ID.");
            return RedirectToAction("Index");
        }

        [HttpGet("remove-item/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();

            var result = await _cartService.RemoveProductFromCart(id, shoppingCartList);
            if (result.Success)
            {
                HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                _notyf.Success("Product removed from cart.");
            }
            else
            {
                _notyf.Error("Failed to remove product from cart.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCart();
            if (result.Success)
            {
                _notyf.Success("Cart cleared successfully.");
            }
            else
            {
                _notyf.Error("Failed to clear the cart.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Sqft = prod.TempSqft });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
