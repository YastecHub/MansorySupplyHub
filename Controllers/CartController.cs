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
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly INotyfService _notyf;

        public CartController(ICartService cartService, INotyfService notyf)
        {
            _cartService = cartService;
            _notyf = notyf;
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
                return View(result.Data);
            }

            _notyf.Error("Failed to load cart items.");
            return View(new List<ProductDto>());
        }

        [HttpGet("cart-summary")]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim?.Value;

            var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();
            var productIds = shoppingCartList.Select(i => i.ProductId).ToList();

            var result = await _cartService.GetUserCartDetails(userId, productIds);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Failed to load cart summary.");
            return RedirectToAction("Index");
        }

        [HttpPost("cart-summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost(ProductUserDto productUserDto)
        {
           
            _notyf.Success("Cart summary processed successfully.");
            return RedirectToAction(nameof(InquiryConfirmation));
        }

        [HttpGet("cart-confirmation")]
        public async Task<IActionResult> InquiryConfirmation()
        {
            var result = await _cartService.ClearCart();
            if (result.Success)
            {
                _notyf.Success("Inquiry Submitted successfully.");
            }
            else
            {
                _notyf.Error("Inquiry failed to submit.");
            }

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
    }
}
