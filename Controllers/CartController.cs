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
            var result = await _cartService.GetCartProductsWithQuantities(HttpContext);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error(result.Message ?? "Failed to load cart items.");
            return View(new List<ProductDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> ProdList)
        {
            _cartService.UpdateSessionCart(ProdList, HttpContext);
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
                _notyf.Error("User not logged in, Please Login or Register here");
                return Redirect("/Identity/Account/Login");

            }

            var result = await _cartService.GetCartSummaryAsync(userId);

            if (!result.Success)
            {
                _notyf.Warning(result.Message); 
                return RedirectToAction("Index");
            }

            var gateway = _brainTreeGate.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            return View(result.Data);
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
                return RedirectToAction("Account", "Login");
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
                string nonceFromTheClient = collection["payment_method_nonce"];
                var orderResponse = await _cartService.CreateOrderAsync(productUserDto, userId, shoppingCartList, nonceFromTheClient);

                if (!orderResponse.Success)
                {
                    _notyf.Error(orderResponse.Message);
                    return RedirectToAction(nameof(Summary));
                }

                return RedirectToAction(nameof(Summary));
            }
            else
            {
                var inquiryResponse = await _cartService.CreateInquiryAsync(productUserDto, userId, shoppingCartList);

                if (!inquiryResponse.Success)
                {
                    _notyf.Error(inquiryResponse.Message);
                    return RedirectToAction(nameof(Summary));
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
                return RedirectToAction(nameof(InquiryConfirmation), new { id = inquiryResponse.Data.Id, isOrder = false });
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
