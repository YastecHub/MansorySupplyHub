using AspNetCoreHero.ToastNotification.Abstractions;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles =WC.AdminRole)]

    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeService _applicationTypeService;
        private readonly INotyfService _notyf;

        public ApplicationTypeController(IApplicationTypeService applicationTypeService , INotyfService notyf)
        {   
            _applicationTypeService = applicationTypeService;
            _notyf = notyf;
        }

        [HttpGet("applicationTypes")]
        public async Task<IActionResult> Index()
        {
            var applicationType = await _applicationTypeService.GetAllApplicationTypes();
            if (applicationType.Success)
            {
                return View(applicationType.Data);
            }
            return View();
        }

        [HttpGet("applicationType/{id}")]
        public async Task<IActionResult> ApplicationType(int id)
        {
            var applicationType = await _applicationTypeService.GetApplicationTypeDetails(id);
            if (applicationType.Success)
            {
                return View(applicationType.Data);
            }
            return RedirectToAction("Index");
        }

        [HttpGet("create-applicationType")]
        public IActionResult CreateApplicationType() =>
             View();

        [HttpPost("create-applicationType")]
        public async Task<IActionResult> CreateApplicationType([FromForm] CreateApplicationTypeDto request)
        {
            var result = await _applicationTypeService.CreateApplicationType(request);
            if (result.Success)
            {
                _notyf.Success("ApplicationType Created Successfully");
                return RedirectToAction("Index");
            }
            _notyf.Error("Failed To Create ApplicationType");
            return RedirectToAction("CreateApplicationType");
        }

        [HttpGet("applicationType/edit/{id}")]
        public async Task<IActionResult> EditApplicationType(int id)
        {
            var response = await _applicationTypeService.GetApplicationTypeDetails(id);
            if (response.Success)
            {

                return View(response.Data);
            }
           
            return RedirectToAction("Index");
        }

        [HttpPost("applicationType/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditApplicationType(int id, [FromForm] UpdateApplicationTypeDto request)
        {
            var result = await _applicationTypeService.EditApplicationType(request, id);
            if (result.Success)
            {
                _notyf.Success("ApplicationType Edited Successfully");
                return RedirectToAction(nameof(Index));
            }
            _notyf.Error("Failed to Edit ApplicationType");
            return RedirectToAction("Index");
        } 

        [HttpGet("category/delete/{id}")]
        public async Task<IActionResult> DeleteApplicationType([FromRoute] int id)
        {
            var result = await _applicationTypeService.DeleteApplicationType(id);
            if (result.Success)
            {
                _notyf.Success("ApplicationType Deleted Successfully");
                return RedirectToAction(nameof(Index));
            }
            _notyf.Error("Failed to delete ApplicationType");
            return RedirectToAction(nameof(Index));
        }
    }
}