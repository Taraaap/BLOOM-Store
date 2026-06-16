using BLOOM.Business.Services;
using BLOOM.Business.Services.IServices;
using BLOOM.DataAccess.Data;
using BLOOM.Models;
using BLOOM.Models.ViewModels;
using BLOOM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Security.Claims;

namespace BLOOM.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleCustomer + "," + SD.RoleEmployee)]

    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;



        [BindProperty]
        public OrderHeader OrderHeader { get; set; }




        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;

        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Details( int orderId)
        {

            OrderHeader = await _orderService.GetOrderByIdAsync(orderId, includeDetails:true, includeUser:true);
            return View(OrderHeader);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderHeaderFromDb = await _orderService.GetOrderByIdAsync(OrderHeader.Id);
            orderHeaderFromDb.Name = OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderHeader.City;
            orderHeaderFromDb.State = OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderHeader.Carrier) && orderHeaderFromDb.OrderStatus == SD.StatusShipped)
            {
                orderHeaderFromDb.Carrier = OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderHeader.TrackingNumber) && orderHeaderFromDb.OrderStatus == SD.StatusShipped)
            {
                orderHeaderFromDb.TrackingNumber = OrderHeader.TrackingNumber;
            }

            await _orderService.UpdateOrderAsync(orderHeaderFromDb);

            TempData["success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
            

        }


        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
        public async Task<IActionResult> UpdateOrderStatus( string status)
        {
            var orderHeader = await _orderService.GetOrderByIdAsync(OrderHeader.Id);
           if(orderHeader== null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }
            string successMessage;

            switch (status)
            {
                case SD.StatusInProcess:
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                    successMessage = "Oder Processign started successfully";
                    break;
                case SD.StatusCancelled:
                case SD.StatusRefuned:
                    try
                    {
                        bool refundIssued = await _orderService.CancelOrderWithRefundAsync(OrderHeader.Id);
                        if (refundIssued)
                        {
                            successMessage = "Order cancelled and refund issued succesfully. Funds will be returned to customer within 5-10 business day.";
                        }
                        else
                        {
                            successMessage = "Order cancelled successfully.(No payment was processed)";
                        }
                    }catch(InvalidOperationException ex)
                    {
                        // Business rule violation(e.g., trying to cancel shipped order)
                        TempData["error"] = ex.Message;
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }catch(Stripe.StripeException ex)
                    {
                        // Refund failed- order is stil cancelled but admin needs to manually refund
                        TempData["error"] = $"Order cancelled but refund failed:{ex.Message}.please process refund manually in stripe dashboard";
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }

                    break;
                case SD.StatusShipped:

                    if(string.IsNullOrEmpty(OrderHeader.Carrier) || string.IsNullOrEmpty(OrderHeader.TrackingNumber))
                    {
                        TempData["error"] = "Please provide both carrier and tracking number.";
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }

                    await _orderService.UpdateOrderStatusAsync(
                        OrderHeader.Id, SD.StatusShipped, orderHeader.Carrier, OrderHeader.TrackingNumber);
                    successMessage = "Oder Shipped successfully";
                    break;

                default:
                    TempData["error"] = "Invalid Status Update.";
                    return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
            }


            TempData["success"] = successMessage;
            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });


        }

        #region API CALLS
        public async Task<IActionResult> GetAll(string status)
        {
            string? userId = null;
            if(!User.IsInRole(SD.RoleAdmin) && !User.IsInRole(SD.RoleEmployee))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }

            var orders = await _orderService.GetAllOrderAsync(userId, status);
            return Json(new { data = orders });
        }


        #endregion


    }
}
