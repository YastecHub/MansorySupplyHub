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
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;

        public CartController(ICartService cartService, INotyfService notyf, IEmailService emailService)
        {
            _cartService = cartService;
            _notyf = notyf;
            _emailService = emailService;
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

                // Set TempSqFt for each product based on the shopping cart
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim?.Value;

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

            _notyf.Error("Failed to load cart summary, Please login into the application");
            return RedirectToAction("Index");
        }
        [HttpPost("cart-summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost(ProductUserDto productUserDto)
        {
            if (ModelState.IsValid)
            {
                // Reload the cart summary view with validation errors
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claim?.Value;

                var shoppingCartSession = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
                var shoppingCartList = shoppingCartSession?.ToList() ?? new List<ShoppingCart>();
                var productIds = shoppingCartList.Select(i => i.ProductId).ToList();

                var result = await _cartService.GetUserCartDetails(userId, productIds);
                if (result.Success)
                {
                    productUserDto.ProductList = result.Data.ProductList;
                    return View("Summary", productUserDto);
                }

                _notyf.Error("Failed to load cart summary. ");
                return RedirectToAction("Index");
            }

            // Process cart summary here...
            _notyf.Success("Cart summary processed successfully.");
            return RedirectToAction(nameof(InquiryConfirmation));
        }


        //[Authorize]
        [HttpGet("cart-confirmation")]
        public async Task<IActionResult> InquiryConfirmation()
        {
            var result = await _cartService.ClearCart();
            if (result.Success)
            {
                _notyf.Success("Inquiry submitted successfully. Your cart has been cleared.");

                // Send email confirmation
                EmailMetadata emailMetadata = new EmailMetadata(
                    "user@example.com",
                    "Inquiry Confirmation",
                    "Your inquiry has been successfully submitted. Thank you for your interest!"
                );

                ////var emailResult = await _emailService.Send(emailMetadata);
                //if (emailResult.Success)
                //{
                //    _notyf.Success("Confirmation email sent successfully.");
                //}
                //else
                //{
                //    _notyf.Error("Failed to send confirmation email.");
                //}
            }
            else
            {
                _notyf.Error("Failed to submit inquiry.");
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
