using Azure.Core;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProducts();
            if (response.Success)
            {
                return View(response.Data);
            }
            return View("Error", new ErrorViewModel { Message = response.Message });
        }

        // GET-UPSERT
        public async Task<IActionResult> UpsertProduct(int? id)
        {
            ViewBag.CategorySelectList = await _productService.GetCategorySelectList();
            ViewBag.ApplicationTypeSelectList = await _productService.GetApplicationTypeSelectList();

            if (id == null)
            {
                return View(new ProductDto());
            }
            else
            {
                var response = await _productService.GetProductDetails(id.Value);
                if (!response.Success)
                {
                    return NotFound();
                }
                return View(response.Data);
            }
        }
        // POST-UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertProduct(ProductDto productDto)
        {

            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (productDto.Id == 0)
            {
                //creating
                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStreams);
                }
                productDto.Image = fileName + extension;

                var createProductDto = new CreateProductDto
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Image = productDto.Image,
                    CategoryId = productDto.CategoryId,
                    ApplicationTypeId = productDto.ApplicationTypeId,
                };

                var response = await _productService.CreateProduct(createProductDto);

                if (response.Success)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            else
            {
                // Updating
                var objFromDb = await _productService.GetProductDetails(productDto.Id);

                if (files.Count > 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    var oldFile = Path.Combine(upload, objFromDb.Data.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productDto.Image = fileName + extension;
                }
                else
                {
                    productDto.Image = objFromDb.Data.Image;
                }

                var updateProductDto = new UpdateProductDto
                {
                    Id = productDto.Id,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Image = productDto.Image,
                    CategoryId = productDto.CategoryId
                };

                var response = await _productService.EditProduct(updateProductDto, productDto.Id);

                if (response.Success)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            ViewBag.CategorySelectList = await _productService.GetCategorySelectList();
            ViewBag.ApplicationTypeSelectList = await _productService.GetApplicationTypeSelectList();
            return View(productDto);

        }
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var response = await _productService.DeleteProduct(id);
            if (response.Success)
            {
                return RedirectToAction("Index");
            }

            return View("Error", new ErrorViewModel { Message = response.Message });
        }
    }
}
