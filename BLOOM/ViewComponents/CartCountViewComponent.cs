using BLOOM.Business.Services.IServices;
using BLOOM.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace BLOOM.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IShoppingCartServices _shoppingCartServices;

        public CartCountViewComponent(IShoppingCartServices shoppingCartServices)
        {
            _shoppingCartServices = shoppingCartServices;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Session.Remove(SD.SessionCart);
                return View(0);
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                HttpContext.Session.Remove(SD.SessionCart);
                return View(0);
            }
            var sessionCount = HttpContext.Session.GetInt32(SD.SessionCart);
            if (sessionCount.HasValue)
            {
                return View(sessionCount.Value);
            }

            var cartCount = await _shoppingCartServices.GetCartCountItemsAsync(claim.Value);
            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);
            return View(cartCount);

        }
    }
}
