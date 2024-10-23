using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                    Sqft = 1  
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
            var deleteHeaderResponse = await _inqHService.DeleteInquiryHeader(InquiryVM.InquiryHeader.Id);

            if (!deleteDetailsResponse.Success || !deleteHeaderResponse.Success)
            {
                TempData[WC.Error] = "Unable to delete inquiry.";
                return RedirectToAction(nameof(Index));
            }

            TempData[WC.Success] = "Inquiry deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetInquiryList()
        {
            var inquiryHeadersResponse = await _inqHService.GetAllInquiryHeaders();

            if (!inquiryHeadersResponse.Success)
            {
                return Json(new { error = "Unable to load inquiry list." });
            }

            var inquiryHeaders = inquiryHeadersResponse.Data;

            var data = inquiryHeaders.Select(inquiryHeader => new
            {
                id = inquiryHeader.Id,
                fullName = inquiryHeader.FullName,
                phoneNumber = inquiryHeader.PhoneNumber,
                email = inquiryHeader.Email
            });

            return Json(new { data });
        }
    }
}
