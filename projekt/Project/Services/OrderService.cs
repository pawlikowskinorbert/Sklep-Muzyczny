using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Helpers;
using Project.Models;

namespace Project.Services
{
	public class OrderService : IOrderService
	{
		private readonly ApplicationDbContext _context;
		
		public OrderService(ApplicationDbContext context)
		{
			_context = context;
		}
		
		
		public async Task<int> CreateOrderAsync(string userId, List<ShoppingCartItem> cartItems)
		{
			var order = new Order 
			{
				UserId = userId,
				TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
				OrderItems = cartItems.Select(item => new OrderItem
				{
					ProductId = item.ProductId,
					ProductName = item.Product.Name,
					Price = item.Product.Price,
					Quantity = item.Quantity
				}).ToList()
			};
			
			_context.Orders.Add(order);
			
			_context.ShoppingCartItems.RemoveRange(cartItems);
			
			await _context.SaveChangesAsync();
			return order.Id;
		}

		public async Task<Order> GetOrderByIdAsync(int orderId)
		{
			return await _context.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.Id ==orderId);
		}

		public async Task<Pagination<Order>> GetOrdersByUserIdAsync(string userId, int pageIndex, int pageSize)
		{
			var query = _context.Orders
						.Where(o => o.UserId == userId)
						.Include(o => o.OrderItems)
						.AsQueryable();
						
			var count = await query.CountAsync();
			
			
			var data = await query
						.Skip((pageIndex -1) *pageSize)
						.Take(pageSize)
						.ToListAsync();
						
			return new Pagination<Order>(pageIndex, pageSize, count, data);
			
		}

        
    }
}