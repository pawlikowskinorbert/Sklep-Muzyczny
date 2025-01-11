using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories;
using Project.Specifications;

namespace Project.Services
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly IGenericRepository<ShoppingCart> _cartRepo;
		private readonly IGenericRepository<ShoppingCartItem> _cartItemRepo;
		private readonly IGenericRepository<Product> _productRepo;
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;


		public ShoppingCartService(IGenericRepository<ShoppingCart> cartRepo, IGenericRepository<ShoppingCartItem> cartItemRepo,
		 IGenericRepository<Product> productRepo, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_cartRepo = cartRepo;
			_cartItemRepo = cartItemRepo;
			_productRepo = productRepo;
			_context = context;
			_httpContextAccessor = httpContextAccessor;

		}

		public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
		{
			var spec = new ShoppingCartWithItemsSpecification(userId);
			return await _cartRepo.GetEntityWithSpec(spec) ?? new ShoppingCart { AppUserId = userId };
		}

		public async Task AddItemToCartForSessionAsync(int productId, int quantity)
		{
			var session = _httpContextAccessor.HttpContext.Session;
			var cartItemsJson = session.GetString("CartItems");

			List<ShoppingCartItem> cartItems;
			if (!string.IsNullOrEmpty(cartItemsJson))
			{
				cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartItemsJson);
			}
			else
			{
				cartItems = new List<ShoppingCartItem>();
			}

			var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);
			if (existingItem != null)
			{
				existingItem.Quantity += quantity;
			}
			else
			{
				cartItems.Add(new ShoppingCartItem
				{
					ProductId = productId,
					Quantity = quantity
				});
			}

			session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
		}
		public async Task AddItemToCartAsync(string userId, int productId, int quantity)
		{
			var cart = await GetCartByUserIdAsync(userId);
			var item = cart.ShoppingCartItems.FirstOrDefault(i => i.ProductId == productId);

			var product = await _productRepo.GetByIdAsync(productId);

			if (product == null)
			{
				throw new KeyNotFoundException("Produkt nie istnieje.");
			}

			if (item != null)
			{
				var newQuantity = item.Quantity + quantity;
				if (newQuantity > product.QuantityInStoct)
				{
					throw new InvalidOperationException("Nie można dodać więcej produktów niż dostępnych w magazynie.");
				}

				item.Quantity = newQuantity;
			}
			else
			{
				// Dodaj nowy produkt do koszyka
				if (quantity > product.QuantityInStoct)
				{
					throw new InvalidOperationException("Nie można dodać więcej produktów niż dostępnych w magazynie.");
				}

				cart.ShoppingCartItems.Add(new ShoppingCartItem
				{
					ProductId = productId,
					Quantity = quantity
				});
			}

			if (cart.Id == 0)
			{
				_cartRepo.Add(cart);
			}

			await _cartRepo.SaveAllAsync();
		}

		public async Task<CartUpdateResult> UpdateItemQuantityAsync(int itemId, int quantity)
		{
			var item = await _context.ShoppingCartItems.FindAsync(itemId);

			if (item == null)
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = "Produkt nie został znaleziony w koszyku."
				};
			}

			var product = await _context.Products.FindAsync(item.ProductId);
			if (product == null)
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = "Produkt nie istnieje."
				};
			}

			if (quantity > product.QuantityInStoct)
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = $"Nie można ustawić ilości większej niż {product.QuantityInStoct}."
				};
			}

			item.Quantity = quantity;
			await _context.SaveChangesAsync();

			return new CartUpdateResult
			{
				Success = true,
				Message = "Ilość została pomyślnie zaktualizowana."
			};
		}

		public async Task RemoveItemAsync(int itemId)
		{
			var item = await _cartItemRepo.GetByIdAsync(itemId);
			if (item != null)
			{
				_cartItemRepo.Remove(item);
				await _cartItemRepo.SaveAllAsync();
			}
		}
		public async Task<List<ShoppingCartItem>> GetCartForSessionAsync()
		{
			var session = _httpContextAccessor.HttpContext.Session;
			var cartItemsJson = session.GetString("CartItems");

			if (string.IsNullOrEmpty(cartItemsJson))
			{
				return new List<ShoppingCartItem>();
			}

			return JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartItemsJson);
		}



		public async Task MergeSessionCartWithUserAsync(string userId)
		{
			var sessionCartItems = await GetCartForSessionAsync();
			var userCart = await GetCartByUserIdAsync(userId);



			foreach (var sessionItem in sessionCartItems)
			{
				var userItem = userCart.ShoppingCartItems.FirstOrDefault(i => i.ProductId == sessionItem.ProductId);
				if (userItem != null)
				{
					userItem.Quantity += sessionItem.Quantity;
				}
				else
				{
					userCart.ShoppingCartItems.Add(new ShoppingCartItem
					{
						ProductId = sessionItem.ProductId,
						Quantity = sessionItem.Quantity
					});
				}
			}

			// Zapisz w bazie
			if (userCart.Id == 0)
			{
				_cartRepo.Add(userCart);
			}

			await _cartRepo.SaveAllAsync();

			// Wyczyść sesję
			_httpContextAccessor.HttpContext.Session.Remove("CartItems");
		}



		public async Task<bool> IsProductInCartAsync(string userId, int productId)
		{
			var cart = await GetCartByUserIdAsync(userId);
			return cart.ShoppingCartItems.Any(i => i.ProductId == productId);
		}


		public async Task<CartUpdateResult> UpdateItemInSessionAsync(int productId, int newQuantity)
		{
			var session = _httpContextAccessor.HttpContext.Session;
			var cartItemsJson = session.GetString("CartItems");
			if (string.IsNullOrEmpty(cartItemsJson))
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = "Koszyk jest pusty."
				};
			}

			var cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartItemsJson);
			var item = cartItems.FirstOrDefault(i => i.ProductId == productId);

			if (item == null)
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = "Produkt nie został znaleziony w koszyku."
				};
			}

			var product = await _context.Products.FindAsync(productId);
			if (product == null)
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = "Produkt nie istnieje."
				};
			}

			if (newQuantity > product.QuantityInStoct)
			{
				return new CartUpdateResult
				{
					Success = false,
					Message = $"Nie można ustawić ilości większej niż {product.QuantityInStoct}."
				};
			}

			// Ustaw nową ilość
			item.Quantity = newQuantity;

			// Zapisz z powrotem do sesji
			session.SetString("CartItems", JsonSerializer.Serialize(cartItems));

			return new CartUpdateResult
			{
				Success = true,
				Message = "Ilość została pomyślnie zaktualizowana."
			};
		}


		public async Task RemoveItemFromSessionAsync(int productId)
		{
			var session = _httpContextAccessor.HttpContext.Session;
			var cartItemsJson = session.GetString("CartItems");
			if (string.IsNullOrEmpty(cartItemsJson)) return;

			var cartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartItemsJson);
			var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
			if (item != null)
			{
				cartItems.Remove(item);
				session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
			}
		}


	}
}