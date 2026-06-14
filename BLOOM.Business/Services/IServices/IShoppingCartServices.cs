using BLOOM.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLOOM.Business.Services.IServices
{
    public interface IShoppingCartServices
    {
        Task<ShoppingCart?> GetCartByIdAsync(int cartId);
        Task<IEnumerable<ShoppingCart>> GetUserCartItemsAsync (String userId);
        Task <int> GetCartCountItemsAsync(String userId);

        Task<ShoppingCart> AddToCartAsync(ShoppingCart cart);
        Task UpdateCartAsync(ShoppingCart cart);
        Task ClearCartAsync(string userId);



    }
}
