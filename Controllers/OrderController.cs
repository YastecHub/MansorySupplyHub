using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Models;
using System.Transactions;
using Braintree;
using MansorySupplyHub.BrainTree;

namespace MansorySupplyHub.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class OrderController : Controller
    {
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IBrainTreeGate _brainTreeGate;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderService orderHeaderService, IOrderDetailService orderDetailService, IBrainTreeGate brainTreeGate)
        {
            _orderHeaderService = orderHeaderService;
            _orderDetailService = orderDetailService;
            _brainTreeGate = brainTreeGate;
        }

        public async Task<IActionResult> Index(string searchName = null, string searchEmail = null, string searchPhone = null, string status = null)
        {
            var response = await _orderHeaderService.GetAllOrderHeaders();

            if (!response.Success)
            {
                return View(new OrderListVM());
            }

            var orderListVM = new OrderListVM()
            {
                OrderHList = response.Data,
                StatusList = WC.listStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i,
                    Value = i
                }).ToList()
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(status) && status != "--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatus.ToLower().Contains(status.ToLower())).ToList();
            }

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
            var response = await _orderHeaderService.GetOrderHeaderDetails(OrderVM.OrderHeader.Id);
            if (!response.Success)
            {
                return NotFound();
            }

            var orderHeader = response.Data;
            orderHeader.OrderStatus = WC.StatusInProcess;

            var updateResponse = await _orderHeaderService.EditOrderHeader(new UpdateOrderHeaderDto
            {
                CreatedByUserId = orderHeader.CreatedByUserId,
                OrderDate = orderHeader.OrderDate,
                ShippingDate = orderHeader.ShippingDate,
                FinalOrderTotal = orderHeader.FinalOrderTotal,
                OrderStatus = orderHeader.OrderStatus,
                PaymentDate = orderHeader.PaymentDate,
                TransactionId = orderHeader.TransactionId,
                PhoneNumber = orderHeader.PhoneNumber,
                StreetAddress = orderHeader.StreetAddress,
                City = orderHeader.City,
                State = orderHeader.State,
                PostalCode = orderHeader.PostalCode,
                FullName = orderHeader.FullName,
                Email = orderHeader.Email
            }, orderHeader.Id);

            if (!updateResponse.Success)
            {
                return BadRequest();
            }

            TempData[WC.Success] = "Order is In Process";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ShipOrder()
        {
            var response = await _orderHeaderService.GetOrderHeaderDetails(OrderVM.OrderHeader.Id);
            if (!response.Success)
            {
                return NotFound();
            }

            var orderHeader = response.Data;
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            var updateResponse = await _orderHeaderService.EditOrderHeader(new UpdateOrderHeaderDto
            {
                CreatedByUserId = orderHeader.CreatedByUserId,
                OrderDate = orderHeader.OrderDate,
                ShippingDate = orderHeader.ShippingDate,
                FinalOrderTotal = orderHeader.FinalOrderTotal,
                OrderStatus = orderHeader.OrderStatus,
                PaymentDate = orderHeader.PaymentDate,
                TransactionId = orderHeader.TransactionId,
                PhoneNumber = orderHeader.PhoneNumber,
                StreetAddress = orderHeader.StreetAddress,
                City = orderHeader.City,
                State = orderHeader.State,
                PostalCode = orderHeader.PostalCode,
                FullName = orderHeader.FullName,
                Email = orderHeader.Email
            }, orderHeader.Id);

            if (!updateResponse.Success)
            {
                return BadRequest();
            }

            TempData[WC.Success] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder()
        {
            //var gateway = _brainTreeGate.GetGateway();
            //Transaction transaction = gateway.Transaction.Find(OrderHeaderDto.TransactionId);
            //if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            //{
            //    //no refund
            //    Result<Transaction> resultvoid = gateway.Transaction.Void(orderHeader.TransactionId);
            //}
            //else
            //{
            //    //refund
            //    Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            //}
            var response = await _orderHeaderService.GetOrderHeaderDetails(OrderVM.OrderHeader.Id);
            if (!response.Success)
            {
                return NotFound();
            }

            var orderHeader = response.Data;

           

            orderHeader.OrderStatus = WC.StatusRefunded;

            var updateResponse = await _orderHeaderService.EditOrderHeader(new UpdateOrderHeaderDto
            {
                CreatedByUserId = orderHeader.CreatedByUserId,
                OrderDate = orderHeader.OrderDate,
                ShippingDate = orderHeader.ShippingDate,
                FinalOrderTotal = orderHeader.FinalOrderTotal,
                OrderStatus = orderHeader.OrderStatus,
                PaymentDate = orderHeader.PaymentDate,
                TransactionId = orderHeader.TransactionId,
                PhoneNumber = orderHeader.PhoneNumber,
                StreetAddress = orderHeader.StreetAddress,
                City = orderHeader.City,
                State = orderHeader.State,
                PostalCode = orderHeader.PostalCode,
                FullName = orderHeader.FullName,
                Email = orderHeader.Email
            }, orderHeader.Id);

            if (!updateResponse.Success)
            {
                return BadRequest();
            }

            TempData[WC.Success] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var response = await _orderHeaderService.GetOrderHeaderDetails(OrderVM.OrderHeader.Id);
            if (!response.Success)
            {
                return NotFound();
            }

            var orderHeaderFromDb = response.Data;
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;

            var updateResponse = await _orderHeaderService.EditOrderHeader(new UpdateOrderHeaderDto
            {
                CreatedByUserId = orderHeaderFromDb.CreatedByUserId,
                OrderDate = orderHeaderFromDb.OrderDate,
                ShippingDate = orderHeaderFromDb.ShippingDate,
                FinalOrderTotal = orderHeaderFromDb.FinalOrderTotal,
                OrderStatus = orderHeaderFromDb.OrderStatus,
                PaymentDate = orderHeaderFromDb.PaymentDate,
                TransactionId = orderHeaderFromDb.TransactionId,
                PhoneNumber = orderHeaderFromDb.PhoneNumber,
                StreetAddress = orderHeaderFromDb.StreetAddress,
                City = orderHeaderFromDb.City,
                State = orderHeaderFromDb.State,
                PostalCode = orderHeaderFromDb.PostalCode,
                FullName = orderHeaderFromDb.FullName,
                Email = orderHeaderFromDb.Email
            }, orderHeaderFromDb.Id);

            if (!updateResponse.Success)
            {
                return BadRequest();
            }

            TempData[WC.Success] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { id = orderHeaderFromDb.Id });
        }
    }
}
