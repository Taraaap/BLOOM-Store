using BLOOM.Business.Services.IServices;
using BLOOM.Models;
using BLOOM.Models.ViewModels;
using BLOOM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BLOOM.Areas.Customers.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IShoppingCartServices _shoppingCartServices;
        private readonly IApplicationUserService _applicationUserService;
        public CartController(IOrderService orderService, IShoppingCartServices shoppingCartServices, IApplicationUserService applicationUserService, IEmailService emailService)
        {
            _orderService = orderService;
            _shoppingCartServices = shoppingCartServices;
            _applicationUserService = applicationUserService;
            _emailService = emailService;
        }


        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartServices.GetUserCartItemsAsync(userId);
            var user=await _applicationUserService.GetUserByIdAsync(userId);

            
             ShoppingCartVM shoppingCartVM = new()
            {
               ShoppingCartList = cartItems,
               OrderHeader = new()
            };
            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;


            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartServices.GetUserCartItemsAsync(userId);

            shoppingCartVM.ShoppingCartList= cartItems;
           
            shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            shoppingCartVM.OrderHeader.OrderDetails = shoppingCartVM.ShoppingCartList.Select(cart => new OrderDetails
            {
                ProductId = cart.ProductId,
                Price = cart.Price,
                Count = cart.Count,
            }).ToList();

            //Create order
            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);
            var user=await _applicationUserService.GetUserByIdAsync(userId);

            await _emailService.SendOrderConfirmationEmailAsync(user.Email,
            shoppingCartVM.OrderHeader.Id, (decimal)shoppingCartVM.OrderHeader.OrderTotal);

           return RedirectToAction("OrderConfirmation" ,new { id=shoppingCartVM.OrderHeader.Id});
            
        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {

            return View(id);
        }


        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _shoppingCartServices.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                if (cart.Count == 100)
                {
                    //donothing
                }
                else
                {
                    cart.Count++;
                    await _shoppingCartServices.UpdateCartAsync(cart);
                    await UpdateCartSessionAsync();

                }

            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _shoppingCartServices.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count--;
                await _shoppingCartServices.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _shoppingCartServices.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count=0;
                await _shoppingCartServices.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            var cart = await _shoppingCartServices.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            if (count <= 1)
            {
                cart.Count = 0;

            }
            else
            {
                if (count >= 100)
                {
                    cart.Count = 100;
                }
                else
                {
                    cart.Count = count;
                }
               
            }
            await _shoppingCartServices.UpdateCartAsync(cart);
            await UpdateCartSessionAsync();
            return Ok(new { success = true });
        }


        private async Task UpdateCartSessionAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var count = await _shoppingCartServices.GetCartCountItemsAsync(userId);
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }

        }
    }
}
