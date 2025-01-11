using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Services
{
	public interface IShoppingCartService
	{
		Task<ShoppingCart> GetCartByUserIdAsync(string userId);
		Task AddItemToCartAsync(string userId, int productId, int quantity);
		Task<CartUpdateResult> UpdateItemQuantityAsync(int itemId, int quantity);
		Task RemoveItemAsync(int itemId);
		Task<bool> IsProductInCartAsync(string userId, int productId);
		Task AddItemToCartForSessionAsync(int productId, int quantity);
		
		Task MergeSessionCartWithUserAsync(string userId);
		Task<List<ShoppingCartItem>> GetCartForSessionAsync();
		Task RemoveItemFromSessionAsync(int productId);
		Task<CartUpdateResult> UpdateItemInSessionAsync(int productId, int newQuantity);


	}
}