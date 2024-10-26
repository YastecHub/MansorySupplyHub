using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using MansorySupplyHub.BrainTree;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class OrderController : Controller
    {
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderService orderHeaderService, IOrderDetailService orderDetailService)
        {
            _orderHeaderService = orderHeaderService;
            _orderDetailService = orderDetailService;
        }

        public async Task<IActionResult> Index(string searchName = null, string searchEmail = null, string searchPhone = null, string status = null)
        {
            var response = await _orderHeaderService.GetAllOrderHeaders();

            if (!response.Success)
            {
                return View(new OrderListVM());
            }

            var orderListVM = new OrderListVM
            {
                OrderHList = response.Data,
                StatusList = WC.listStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i,
                    Value = i
                }).ToList()
            };
            var filterResponse = await _orderHeaderService.FilterOrderHeadersAsync(orderListVM.OrderHList, searchName, searchEmail, searchPhone, status);

            if (!filterResponse.Success)
            {
                return View(orderListVM);
            }

            orderListVM.OrderHList = filterResponse.Data.ToList();

            return View(orderListVM);
        }



        public async Task<IActionResult> Details(int id)
        {
            var headerResponse = await _orderHeaderService.GetOrderHeaderDetails(id);
            if (!headerResponse.Success)
            {
                return NotFound();
            }

            var detailResponse = await _orderDetailService.GetOrderDetailsByHeaderId(id);
            if (!detailResponse.Success)
            {
                return NotFound();
            }

            OrderVM = new OrderVM()
            {
                OrderHeader = headerResponse.Data,
                OrderDetail = detailResponse.Data
            };

            return View(OrderVM);
        }

        [HttpPost]
        public async Task<IActionResult> StartProcessing()
        {
            var response = await _orderHeaderService.StartProcessingOrder(OrderVM.OrderHeader.Id);

            if (!response.Success)
            {
                return response.Data ? (IActionResult)BadRequest() : NotFound();
            }

            TempData[WC.Success] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ShipOrder()
        {
            var response = await _orderHeaderService.ShipOrder(OrderVM.OrderHeader.Id);

            if (!response.Success)
            {
                return response.Data ? (IActionResult)BadRequest() : NotFound();
            }

            TempData[WC.Success] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder()
        {
            var response = await _orderHeaderService.CancelOrder(OrderVM.OrderHeader.Id);

            if (!response.Success)
            {
                return response.Data ? (IActionResult)BadRequest() : NotFound();
            }

            TempData[WC.Success] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var response = await _orderHeaderService.UpdateOrderDetails(OrderVM);

            if (!response.Success)
            {
                return response.Data ? (IActionResult)BadRequest() : NotFound();
            }

            TempData[WC.Success] = response.Message;
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
    }
}
