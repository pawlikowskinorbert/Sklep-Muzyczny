using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Project.Data;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> _logger;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ApplicationDbContext _context;
		private readonly IShoppingCartService _shoppingCartService;

		public AccountController(ILogger<AccountController> logger, UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager, ApplicationDbContext context, IShoppingCartService shoppingCartService)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_shoppingCartService = shoppingCartService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new AppUser
				{
					UserName = model.UserName,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName
				};

				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user, "User");
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Index", "Home");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

			}
			return View(model);
		}

		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
					if (!user.IsActive)
					{
						ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact the administrator.");
						return View(model);
					}
					
					var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
					if (result.Succeeded)
					{

						Console.WriteLine("Logowanie zakończone sukcesem.");
						await _shoppingCartService.MergeSessionCartWithUserAsync(user.Id);
						if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
						{
							return Redirect(returnUrl);
						}
						else
						{
							return RedirectToAction("Index", "Home");
						}
					}
				}

				ModelState.AddModelError("", "Invalid login attempt.");
			}

			return View(model);
		}
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
		private async Task MigrateCartAsync(string sessionId, string userId)
		{
			// Pobierz koszyk gościa (powiązany z sesją)
			var guestCart = await _context.ShoppingCarts
				.Include(c => c.ShoppingCartItems)
				.FirstOrDefaultAsync(c => c.AppUserId == sessionId);

			if (guestCart != null)
			{
				// Pobierz koszyk użytkownika (jeśli istnieje)
				var userCart = await _context.ShoppingCarts
					.Include(c => c.ShoppingCartItems)
					.FirstOrDefaultAsync(c => c.AppUserId == userId);

				if (userCart == null)
				{
					// Jeśli użytkownik nie ma koszyka, stwórz nowy
					userCart = new ShoppingCart
					{
						AppUserId = userId,
						ShoppingCartItems = new List<ShoppingCartItem>()
					};
					_context.ShoppingCarts.Add(userCart);
				}

				// Migruj elementy z koszyka gościa do koszyka użytkownika
				foreach (var item in guestCart.ShoppingCartItems)
				{
					var existingItem = userCart.ShoppingCartItems
						.FirstOrDefault(i => i.ProductId == item.ProductId);

					if (existingItem == null)
					{
						// Jeśli produktu nie ma w koszyku użytkownika, dodaj go
						userCart.ShoppingCartItems.Add(new ShoppingCartItem
						{
							ProductId = item.ProductId,
							Quantity = item.Quantity,
							MaxQuantity = item.MaxQuantity
						});
					}
					else
					{
						// Jeśli produkt już istnieje w koszyku użytkownika, zaktualizuj ilość
						existingItem.Quantity += item.Quantity;
					}
				}

				// Usuń koszyk gościa
				_context.ShoppingCarts.Remove(guestCart);
				await _context.SaveChangesAsync();
			}
		}
	}
}