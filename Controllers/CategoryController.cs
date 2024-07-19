using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> Index()
        {
            var category = await _categoryService.GetAllCategories();
            if (category.Success)
            {
                return View(category.Data);
            }

            return View();
        }

        [HttpGet("category/{id}")]
        public async Task<IActionResult> Category(int id)
        {
            var category = await _categoryService.GetCategoryDetails(id);
            if (category.Success)
            {
                return View(category.Data);
            }
            return RedirectToAction("Index");
        }

        [HttpGet("create-category")]
        public IActionResult CreateCategory() =>
             View();

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto request)
        {
            var result = await _categoryService.CreateCategory(request);
            if (result.Success)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("CreateCategory");
        }

        [HttpGet("category/edit/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var response = await _categoryService.GetCategoryDetails(id);
            if (response.Success)
            {
                return View(response.Data);
            }
            return RedirectToAction("Index");
        }

        [HttpPost("category/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [FromForm] UpdateCategoryDto request)
        {
            var result = await _categoryService.EditCategory(request, id);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }


        [HttpGet("category/delete/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
