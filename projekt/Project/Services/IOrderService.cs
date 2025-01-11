using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Helpers;
using Project.Models;

namespace Project.Services
{
	public interface IOrderService
	{
		Task<int> CreateOrderAsync(string userId, List<ShoppingCartItem> cartItems);
		Task<Order> GetOrderByIdAsync(int orderId);
		Task<Pagination<Order>> GetOrdersByUserIdAsync(string userId, int pageIndex, int pageSize);
	}
}