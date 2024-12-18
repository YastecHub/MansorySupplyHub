﻿using AspNetCoreHero.ToastNotification.Abstractions;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly INotyfService _notyf;

        public CategoryController(ICategoryService categoryService, INotyfService notyf)
        {
            _categoryService = categoryService;
            _notyf = notyf;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> Index()
        {
            var result = await _categoryService.GetAllCategories();
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Failed to load categories.");
            return View();
        }

        [HttpGet("category/{id}")]
        public async Task<IActionResult> Category(int id)
        {
            var result = await _categoryService.GetCategoryDetails(id);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Category not found.");
            return RedirectToAction("Index");
        }

        [HttpGet("create-category")]
        public IActionResult CreateCategory() => View();

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto request)
        {
            var result = await _categoryService.CreateCategory(request);
            if (result.Success)
            {
                _notyf.Success("Category created successfully.");
                return RedirectToAction("Index");
            }

            _notyf.Error("Failed to create category.");
            return RedirectToAction("CreateCategory");
        }

        [HttpGet("category/edit/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var result = await _categoryService.GetCategoryDetails(id);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Category not found.");
            return RedirectToAction("Index");
        }

        [HttpPost("category/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [FromForm] UpdateCategoryDto request)
        {
            var result = await _categoryService.EditCategory(request, id);
            if (result.Success)
            {
                _notyf.Success("Category edited successfully.");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("Failed to edit category.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("category/delete/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (result.Success)
            {
                _notyf.Success("Category deleted successfully.");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("Failed to delete category.");
            return RedirectToAction(nameof(Index));
        }
    }
}
