using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Data;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{

	public class OrderController : Controller
	{
		private readonly IOrderService _orderService;
		private readonly IShoppingCartService _cartService;
		private readonly ApplicationDbContext _context;

		public OrderController(IOrderService orderService, IShoppingCartService cartService, ApplicationDbContext context)
		{
			_orderService = orderService;
			_cartService = cartService;
			_context = context;
		}

		[HttpGet]
		public IActionResult PlaceOrder()
		{
			if (!User.Identity.IsAuthenticated)
			{
				TempData["Error"] = "Musisz być zalogowany, aby złożyć zamówienie.";
				return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "ShoppingCart") });
			}

			return RedirectToAction("FinalizeOrder");
		}

		[HttpPost]
		public async Task<IActionResult> FinalizeOrder()
		{
			if (!User.Identity.IsAuthenticated)
			{
				TempData["Error"] = "Musisz się zalogować, aby sfinalizować zamówienie.";
				return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "ShoppingCart") });
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


			var cart = await _context.ShoppingCarts
				.Include(c => c.ShoppingCartItems)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(c => c.AppUserId == userId);

			if (cart == null || !cart.ShoppingCartItems.Any())
			{
				TempData["Error"] = "Twój koszyk jest pusty!";
				return RedirectToAction("Index", "ShoppingCart");
			}

			var order = new Order
			{
				UserId = userId,
				TotalAmount = cart.ShoppingCartItems.Sum(i => i.Quantity * i.Product.Price),
				OrderItems = cart.ShoppingCartItems.Select(i => new OrderItem
				{
					ProductId = i.ProductId,
					ProductName = i.Product.Name,
					Price = i.Product.Price,
					Quantity = i.Quantity
				}).ToList(),
				CreatedAt = DateTime.UtcNow
			};

			foreach (var item in cart.ShoppingCartItems)
			{
				var product = item.Product;

				if (product.QuantityInStoct < item.Quantity)
				{
					TempData["Error"] = $"Produkt \"{product.Name}\" nie ma wystarczającej ilości w magazynie.";
					return RedirectToAction("Index", "ShoppingCart");
				}

				product.QuantityInStoct -= item.Quantity;
				_context.Products.Update(product); // Aktualizacja produktu
			}

			_context.Orders.Add(order);

			_context.ShoppingCartItems.RemoveRange(cart.ShoppingCartItems);


			await _context.SaveChangesAsync();

			TempData["Success"] = "Zamówienie zostało pomyślnie złożone";
			return RedirectToAction(nameof(Details), new { id = order.Id });
		}
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			// Pobierz szczegóły zamówienia
			var order = await _context.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.Id == id);

			if (order == null)
			{
				TempData["Error"] = "Nie znaleziono zamówienia.";
				return RedirectToAction("Index", "Product");
			}

			return View(order);
		}


		[HttpGet("Order/MyOrders")]
		public async Task<IActionResult> GetUserOrders(int pageIndex = 1, int pageSize = 10)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId)) return Unauthorized();

			var orders = await _orderService.GetOrdersByUserIdAsync(userId, pageIndex, pageSize);
			return View("MyOrders", orders);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}

	}

}