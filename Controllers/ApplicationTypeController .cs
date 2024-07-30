using AspNetCoreHero.ToastNotification.Abstractions;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeService _applicationTypeService;
        private readonly INotyfService _notyf;

        public ApplicationTypeController(IApplicationTypeService applicationTypeService, INotyfService notyf)
        {
            _applicationTypeService = applicationTypeService;
            _notyf = notyf;
        }

        [HttpGet("applicationTypes")]
        public async Task<IActionResult> Index()
        {
            var result = await _applicationTypeService.GetAllApplicationTypes();
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Failed to load application types.");
            return View();
        }

        [HttpGet("applicationType/{id}")]
        public async Task<IActionResult> ApplicationType(int id)
        {
            var result = await _applicationTypeService.GetApplicationTypeDetails(id);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Failed to load application type details.");
            return RedirectToAction("Index");
        }

        [HttpGet("create-applicationType")]
        public IActionResult CreateApplicationType()
        {
            return View();
        }

        [HttpPost("create-applicationType")]
        public async Task<IActionResult> CreateApplicationType([FromForm] CreateApplicationTypeDto request)
        {
            var result = await _applicationTypeService.CreateApplicationType(request);
            if (result.Success)
            {
                _notyf.Success("Application type created successfully.");
                return RedirectToAction("Index");
            }

            _notyf.Error("Failed to create application type.");
            return RedirectToAction("CreateApplicationType");
        }

        [HttpGet("applicationType/edit/{id}")]
        public async Task<IActionResult> EditApplicationType(int id)
        {
            var result = await _applicationTypeService.GetApplicationTypeDetails(id);
            if (result.Success)
            {
                return View(result.Data);
            }

            _notyf.Error("Failed to load application type details.");
            return RedirectToAction("Index");
        }

        [HttpPost("applicationType/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditApplicationType(int id, [FromForm] UpdateApplicationTypeDto request)
        {
            var result = await _applicationTypeService.EditApplicationType(request, id);
            if (result.Success)
            {
                _notyf.Success("Application type edited successfully.");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("Failed to edit application type.");
            return RedirectToAction("Index");
        }

        [HttpGet("applicationType/delete/{id}")]
        public async Task<IActionResult> DeleteApplicationType([FromRoute] int id)
        {
            var result = await _applicationTypeService.DeleteApplicationType(id);
            if (result.Success)
            {
                _notyf.Success("Application type deleted successfully.");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("Failed to delete application type.");
            return RedirectToAction(nameof(Index));
        }
    }
}
