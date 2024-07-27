using MansorySupplyHub.Dto;
using MansorySupplyHub.Entities;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using MansorySupplyHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderService _inquiryHeaderService;
        private readonly IInquiryDetailService _inquiryDetailService;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderService inquiryHeaderService, IInquiryDetailService inquiryDetailService)
        {
            _inquiryHeaderService = inquiryHeaderService;
            _inquiryDetailService = inquiryDetailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInquiryList()
        {
            var inquiryHeadersResponse = await _inquiryHeaderService.GetAllInquiryHeadersAsync();
            if (inquiryHeadersResponse.Success)
            {
                return Json(new { data = inquiryHeadersResponse.Data });
            }
            return Json(new { data = new List<InquiryHeaderDto>() });
        }

        //public async Task<IActionResult> Details(int id)
        //{
        //    var inquiryHeaderResponse = await _inquiryHeaderService.GetInquiryHeaderByIdAsync(id);
        //    var inquiryDetailResponse = await _inquiryDetailService.GetInquiryDetailsByHeaderIdAsync(inquiryHeaderResponse.Data?.Id ?? 0);

        //    if (!inquiryHeaderResponse.Success || !inquiryDetailResponse.Success)
        //    {
        //        TempData[WC.Error] = "Error loading inquiry details.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    InquiryVM = new InquiryVM
        //    {
        //        InquiryHeader = inquiryHeaderResponse.Data,
        //        InquiryDetail = inquiryDetailResponse.Data
        //    };

        //    return View(InquiryVM);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Details()
        //{
        //    List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        //    var inquiryDetailsResponse = await _inquiryDetailService.GetInquiryDetailsByHeaderIdAsync(InquiryVM.InquiryHeader.Id);

        //    if (!inquiryDetailsResponse.Success)
        //    {
        //        TempData[WC.Error] = "Error loading inquiry details.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    foreach (var detail in inquiryDetailsResponse.Data)
        //    {
        //        ShoppingCart shoppingCart = new ShoppingCart
        //        {
        //            ProductId = detail.ProductId,
        //            SqFt = 1
        //        };
        //        shoppingCartList.Add(shoppingCart);
        //    }

        //    HttpContext.Session.Clear();
        //    HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
        //    HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);
        //    return RedirectToAction("Index", "Cart");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var deleteHeaderResponse = await _inquiryHeaderService.DeleteInquiryHeaderAsync(id);
        //    var deleteDetailsResponse = await _inquiryDetailService.DeleteInquiryDetailsByHeaderIdAsync(id);

        //    if (!deleteHeaderResponse.Success || !deleteDetailsResponse.Success)
        //    {
        //        TempData[WC.Error] = "Error deleting inquiry.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    TempData[WC.Success] = "Inquiry deleted successfully.";
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
