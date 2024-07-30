﻿using AspNetCoreHero.ToastNotification.Abstractions;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MansorySupplyHub.Utility;

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

            if (id == null)
            {
                return View(new ProductDto());
            }
            else
            {
                var response = await _productService.GetProductDetails(id.Value);
                if (!response.Success)
                {
                    _notyf.Error("Product not found");
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
                // Creating
                if (files.Count > 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        await files[0].CopyToAsync(fileStreams);
                    }
                    productDto.Image = fileName + extension;
                }

                var createProductDto = new CreateProductDto
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Image = productDto.Image,
                    ApplicationType = productDto.ApplicationType,
                    Category = productDto.Category,
                    CategoryId = productDto.CategoryId,
                    ApplicationTypeId = productDto.ApplicationTypeId,
                };
                var response = await _productService.CreateProduct(createProductDto);

                if (response.Success)
                {
                    _notyf.Success("Product created successfully");
                    return RedirectToAction("Index");
                }
                _notyf.Error("Failed to create product");
                return RedirectToAction("UpsertProduct");
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
                        await files[0].CopyToAsync(fileStream);
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
                    Category = productDto.Category,
                    ApplicationType = productDto.ApplicationType,
                    CategoryId = productDto.CategoryId,
                    ApplicationTypeId = productDto.ApplicationTypeId,
                };
                ViewBag.CategorySelectList = await _productService.GetCategorySelectList();
                ViewBag.ApplicationTypeSelectList = await _productService.GetApplicationTypeSelectList();

                var response = await _productService.EditProduct(updateProductDto, productDto.Id);

                if (response.Success)
                {
                    _notyf.Success("Product updated successfully");
                    return RedirectToAction("Index");
                }
                _notyf.Error("Failed to update product");
                return RedirectToAction("UpsertProduct");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
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
