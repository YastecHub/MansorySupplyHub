using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using MansorySupplyHub.Utility;
using MansorySupplyHub.Entities;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderService _inqHService;
        private readonly IInquiryDetailService _inqDService;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderService inqHService, IInquiryDetailService inqDService)
        {
            _inqHService = inqHService;
            _inqDService = inqDService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var inquiryHeaderResponse = await _inqHService.GetInquiryHeaderDetails(id);
            var inquiryDetailsResponse = await _inqDService.GetInquiryDetailsByHeaderId(id);

            if (!inquiryHeaderResponse.Success || !inquiryDetailsResponse.Success)
            {
                TempData[WC.Error] = "Unable to load inquiry details.";
                return RedirectToAction(nameof(Index));
            }

            InquiryVM inquiryVM = new InquiryVM()
            {
                InquiryHeader = inquiryHeaderResponse.Data,
                InquiryDetail = inquiryDetailsResponse.Data
            };

            return View(inquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details()
        {
            var inquiryDetailsResponse = await _inqDService.GetInquiryDetailsByHeaderId(InquiryVM.InquiryHeader.Id);

            if (!inquiryDetailsResponse.Success)
            {
                TempData[WC.Error] = "Unable to load inquiry details.";
                return RedirectToAction(nameof(Index));
            }

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var detail in inquiryDetailsResponse.Data)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                    Sqft = 1  // Assuming a default value for demonstration
                };
                shoppingCartList.Add(shoppingCart);
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            var deleteDetailsResponse = await _inqDService.DeleteInquiryDetailsByHeaderId(InquiryVM.InquiryHeader.Id);
            if (!deleteDetailsResponse.Success)
            {
                TempData[WC.Error] = "Unable to delete the inquiry details.";
                return RedirectToAction(nameof(Index));
            }

            var deleteHeaderResponse = await _inqHService.DeleteInquiryHeader(InquiryVM.InquiryHeader.Id);
            if (!deleteHeaderResponse.Success)
            {
                TempData[WC.Error] = "Unable to delete the inquiry header.";
                return RedirectToAction(nameof(Index));
            }

            TempData[WC.Success] = "Inquiry deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetInquiryList()
        {
            var inquiryHeadersResponse = await _inqHService.GetAllInquiryHeaders();
            if (!inquiryHeadersResponse.Success)
            {
                return Json(new { success = false, message = "Failed to load inquiry headers." });
            }
            return Json(new { data = inquiryHeadersResponse.Data });
        }
        #endregion
    }
}
