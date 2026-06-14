using BLOOM.Business.Services.IServices;
using BLOOM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BLOOM.Areas.Customers.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartServices _shoppingCartServices;
        public HomeController(IProductService productService, IShoppingCartServices shoppingCartServices)
        {
            _productService = productService;
            _shoppingCartServices = shoppingCartServices;
        }


        public async Task<IActionResult> Index()
        { 
            var products= await _productService.GetAllProductsAsync(includeCategory:true);
            return View(products);
        }


        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId, includeCategory:true);

            if (product == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new()
            {
                Product = product,
                Count = 1,
                ProductId = productId
            };

            return View(cart);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            shoppingCart.ApplicationUserId = userId;
            await _shoppingCartServices.AddToCartAsync(shoppingCart);

            TempData["success"] = "Item added to cart";
            return RedirectToAction("Index");
        }


    }
}
