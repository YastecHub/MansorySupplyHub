using AspNetCoreHero.ToastNotification.Abstractions;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MansorySupplyHub.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IInquiryHeaderService _inqHService;
        private readonly IInquiryDetailService _inqDService;
        private readonly IOrderHeaderService _orderHService;
        private readonly IOrderDetailService _orderDService;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;

        public CartController(ICartService cartService, IInquiryHeaderService inqHService, IInquiryDetailService inqDService, IOrderHeaderService orderHService, IOrderDetailService orderDService, INotyfService notyf, IEmailService emailService)
        {
            _cartService = cartService;
            _inqHService = inqHService;
            _inqDService = inqDService;
            _orderHService = orderHService;
            _orderDService = orderDService;
            _notyf = notyf;
            _emailService = emailService;
        }

        [HttpGet("cart-items")]
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
        public IActionResult IndexPost(IEnumerable<ProductDto> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (ProductDto prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Sqft = prod.TempSqft });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Summary));
        }

        [HttpGet("cart-summary")]
        public async Task<IActionResult> Summary()
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
            var productIds = shoppingCartList.Select(i => i.ProductId).ToList();

            if (!shoppingCartList.Any())
            {
                _notyf.Warning("Your cart is empty.");
                return RedirectToAction("Index");
            }

            var result = await _cartService.GetUserCartDetails(userId, productIds);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Failed to load cart summary. Please try again.");
            return RedirectToAction("Index");
        }

        [HttpPost("cart-summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost(ProductUserDto productUserDto)
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

            if (User.IsInRole(WC.AdminRole))
            {
                // Create an order
                var orderHeaderDto = new CreateOrderHeaderDto
                {
                    CreatedByUserId = userId,
                    FinalOrderTotal = productUserDto.ProductList.Sum(p => p.TempSqft * p.Price),
                    FullName = productUserDto.ApplicationUser.FullName,
                    PhoneNumber = productUserDto.ApplicationUser.PhoneNumber,
                    Email = productUserDto.ApplicationUser.Email,
                    StreetAddress = productUserDto.ApplicationUser.StreetAddress,
                    City = productUserDto.ApplicationUser.City,
                    State = productUserDto.ApplicationUser.State,
                    PostalCode = productUserDto.ApplicationUser.PostalCode,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };

                var orderHeaderResponse = await _orderHService.CreateOrderHeader(orderHeaderDto);
                if (!orderHeaderResponse.Success)
                {
                    _notyf.Error("Failed to create order.");
                    return RedirectToAction(nameof(Summary));
                }

                foreach (var cartItem in shoppingCartList)
                {
                    var orderDetailDto = new CreateOrderDetailDto
                    {
                        OrderHeaderId = orderHeaderResponse.Data.Id,
                        ProductId = cartItem.ProductId,
                        Sqft = cartItem.Sqft,
                        PricePerSqFt = productUserDto.ProductList.First(p => p.Id == cartItem.ProductId).Price
                    };

                    var orderDetailResponse = await _orderDService.CreateOrderDetail(orderDetailDto);
                    if (!orderDetailResponse.Success)
                    {
                        _notyf.Error("Failed to create order detail.");
                        return RedirectToAction(nameof(Summary));
                    }
                }

                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeaderResponse.Data.Id });
            }
            else
            {
                // Create an inquiry
                var inquiryHeaderDto = new CreateInquiryHeaderDto
                {
                    ApplicationUserId = userId,
                    InquiryDate = DateTime.Now,
                    FullName = productUserDto.ApplicationUser.FullName,
                    PhoneNumber = productUserDto.ApplicationUser.PhoneNumber,
                    Email = productUserDto.ApplicationUser.Email
                };

                var inquiryHeaderResponse = await _inqHService.CreateInquiryHeader(inquiryHeaderDto);
                if (!inquiryHeaderResponse.Success)
                {
                    _notyf.Error("Failed to create inquiry header.");
                    return RedirectToAction(nameof(Summary));
                }

                var inquiryHeaderId = inquiryHeaderResponse.Data.Id;

                foreach (var cartItem in shoppingCartList)
                {
                    var inquiryDetailDto = new CreateInquiryDetailDto
                    {
                        InquiryHeaderId = inquiryHeaderId,
                        ProductId = cartItem.ProductId,
                        Sqft = cartItem.Sqft
                    };

                    var inquiryDetailResponse = await _inqDService.CreateInquiryDetail(inquiryDetailDto);
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
                return RedirectToAction(nameof(InquiryConfirmation));
            }
        }

        [HttpGet("cart-confirmation")]
        public async Task<IActionResult> InquiryConfirmation()
        {
            _notyf.Success("Your inquiry has been successfully submitted.");
            return View();
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