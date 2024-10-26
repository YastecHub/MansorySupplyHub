using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using MansorySupplyHub.Utility;

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
            var inquiryDetailsResponse = await _inqHService.GetInquiryDetailsVM(id);

            if (!inquiryDetailsResponse.Success)
            {
                TempData[WC.Error] = inquiryDetailsResponse.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(inquiryDetailsResponse.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details()
        {
            var cartResponse = await _inqDService.ConvertInquiryToCart(InquiryVM.InquiryHeader.Id);

            if (!cartResponse.Success)
            {
                TempData[WC.Error] = cartResponse.Message;
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, cartResponse.Data);
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

            var data = inquiryHeadersResponse.Data.Select(inquiryHeader => new
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
