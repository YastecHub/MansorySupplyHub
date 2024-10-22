using AspNetCoreHero.ToastNotification.Abstractions;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;

        public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProducts();
            if (response.Success)
            {
                return View(response.Data);
            }
            _notyf.Error("Failed to load products");
            return View(new List<ProductDto>());
        }

        // GET-UPSERT
        public async Task<IActionResult> UpsertProduct(int? id)
        {
            ViewBag.CategorySelectList = await _productService.GetCategorySelectList();
            ViewBag.ApplicationTypeSelectList = await _productService.GetApplicationTypeSelectList();

            var response = await _productService.GetProductForUpsert(id);
            if (!response.Success)
            {
                _notyf.Error(response.Message);
                return NotFound();
            }
            return View(response.Data);
        }

        // POST-UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertProduct(ProductDto productDto)
        {
            var response = await _productService.UpsertProduct(productDto, HttpContext.Request.Form.Files, _webHostEnvironment.WebRootPath);

            if (response.Success)
            {
                _notyf.Success(response.Message);
                return RedirectToAction("Index");
            }
            _notyf.Error(response.Message);
            return View(productDto);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProduct(id);
            if (response.Success)
            {
                _notyf.Success("Product deleted successfully");
                return RedirectToAction("Index");
            }
            _notyf.Error("Failed to delete product");
            return View();
        }
    }
}
