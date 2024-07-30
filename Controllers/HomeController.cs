using System.Diagnostics;
using MansorySupplyHub.Data;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MansorySupplyHub.Utility;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace MansorySupplyHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, INotyfService notyf)
        {
            _logger = logger;
            _db = db;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            HomeViewModel homeVM = new HomeViewModel()
            {
                Products = _db.Products.Include(u => u.Category).Include(u => u.ApplicationType),
                Categories = _db.Categories
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            DetailsViewModel detailsViewModel = new DetailsViewModel()
            {
                Product = _db.Products.Include(u => u.Category).Include(u => u.ApplicationType)
                .Where(u => u.Id == id).FirstOrDefault(),
                ExistsInCart = shoppingCartList.Any(item => item.ProductId == id)
            };

            return View(detailsViewModel);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var productInCart = shoppingCartList.SingleOrDefault(item => item.ProductId == id);
            if (productInCart == null)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = id });
                HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                _notyf.Success("Product added to cart successfully.");
            }
            else
            {
                _notyf.Error("Product is already in the cart.");
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
                HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                _notyf.Success("Product removed from cart successfully.");
            }
            else
            {
                _notyf.Error("Product not found in cart.");
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
