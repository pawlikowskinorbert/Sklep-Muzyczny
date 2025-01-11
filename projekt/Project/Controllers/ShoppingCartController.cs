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
	public class ShoppingCartController : Controller
	{

		private readonly IShoppingCartService _cartService;
		private readonly ApplicationDbContext _context;

		public ShoppingCartController(IShoppingCartService cartService, ApplicationDbContext context)
		{
			_cartService = cartService;
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


			if (string.IsNullOrEmpty(userId))
			{

				var sessionItems = await _cartService.GetCartForSessionAsync();

				var productIds = sessionItems.Select(i => i.ProductId).Distinct().ToList();

				var products = await _context.Products
					.Where(p => productIds.Contains(p.Id))
					.ToListAsync();

				var productsDict = products.ToDictionary(p => p.Id, p => p);

				var viewModel = new ShoppingCartViewModel
				{
					Items = sessionItems.Select(sessionItem =>
					{
						var product = productsDict[sessionItem.ProductId];

						return new ShoppingCartItemViewModel
						{
							ProductId = sessionItem.ProductId,
							Id = 0,
							ProductName = product.Name,
							Price = product.Price,
							Quantity = sessionItem.Quantity,
							TotalPrice = sessionItem.Quantity * product.Price
						};
					}).ToList()
				};

				return View(viewModel);
			}
			else
			{
				var cart = await _cartService.GetCartByUserIdAsync(userId);

				var viewModel = new ShoppingCartViewModel
				{
					Items = cart.ShoppingCartItems.Select(i => new ShoppingCartItemViewModel
					{

						Id = i.Id,
						ProductName = i.Product.Name,
						Price = i.Product.Price,
						Quantity = i.Quantity,
						TotalPrice = i.Quantity * i.Product.Price
					}).ToList()
				};

				return View(viewModel);
			}
		}



		[HttpPost]
		public async Task<IActionResult> Add(int productId, int quantity)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				// Niezalogowany -> dodaj do koszyka w sesji
				await _cartService.AddItemToCartForSessionAsync(productId, quantity);
				// Zamiast przekierowania - zwróć 200
				return Ok(new { message = "Produkt został dodany do koszyka (sesja)." });
			}
			else
			{
				// Zalogowany -> dodaj do koszyka w bazie
				try
				{
					var isInCart = await _cartService.IsProductInCartAsync(userId, productId);
					if (isInCart)
					{
						// Conflict() -> HTTP 409 (zasób już istnieje)
						return Conflict(new { error = "Produkt już znajduje się w koszyku." });
					}

					await _cartService.AddItemToCartAsync(userId, productId, quantity);
					// Zwróć 200
					return Ok(new { message = "Produkt został dodany do koszyka (baza)." });
				}
				catch (Exception ex)
				{
					// Błąd -> HTTP 400/500
					return BadRequest(new { error = ex.Message });
				}
			}
		}



		[HttpGet]
		public async Task<IActionResult> GetCartItemCount()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			int cartItemCount = 0;
			if (!string.IsNullOrEmpty(userId))
			{
				// Użytkownik zalogowany – koszyk z bazy
				var cart = await _cartService.GetCartByUserIdAsync(userId);
				cartItemCount = cart.ShoppingCartItems.Sum(item => item.Quantity);
			}
			else
			{
				// Użytkownik niezalogowany – koszyk z sesji
				var sessionCartItems = await _cartService.GetCartForSessionAsync();
				cartItemCount = sessionCartItems.Sum(item => item.Quantity);
			}

			return Json(new { cartItemCount });
		}


		[HttpPost]
		public async Task<IActionResult> Update(int itemId, int productId, int quantity)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				// Niezalogowany -> aktualizujemy w sesji po productId
				var result = await _cartService.UpdateItemInSessionAsync(productId, quantity);

				if (!result.Success)
				{
					TempData["Error"] = result.Message;
				}
				else
				{
					TempData["Success"] = "Zaktualizowano ilość w koszyku (sesja).";
				}

				return RedirectToAction(nameof(Index));
			}
			else
			{
				// Zalogowany -> aktualizujemy w bazie po itemId
				var result = await _cartService.UpdateItemQuantityAsync(itemId, quantity);
				if (!result.Success)
				{
					TempData["Error"] = result.Message;
				}
				else
				{
					TempData["Success"] = "Ilość zaktualizowana w bazie.";
				}
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public async Task<IActionResult> Remove(int itemId, int productId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				// Niezalogowany -> usuwamy z sesji po productId
				await _cartService.RemoveItemFromSessionAsync(productId);
				TempData["Success"] = "Usunięto produkt z koszyka (sesja).";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				// Zalogowany -> usuwamy z bazy po itemId
				await _cartService.RemoveItemAsync(itemId);
				TempData["Success"] = "Usunięto produkt z koszyka (baza).";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpGet("error")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
	}
}