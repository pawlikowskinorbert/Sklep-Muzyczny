using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Models;

namespace Project.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UserController(UserManager<AppUser> userManager) : Controller
	{



		public async Task<IActionResult> Index()
		{
			var users = await userManager.Users
		.Select(u => new UserViewModel
		{
			Id = u.Id,
			UserName = u.UserName,
			Email = u.Email,
			IsActive = u.IsActive,
		})
		.ToListAsync();

			foreach (var user in users)
			{
				var roles = await userManager.GetRolesAsync(await userManager.FindByIdAsync(user.Id));
				user.Roles = string.Join(", ", roles);
			}


			return View(users);
		}


		[HttpPost]
		public async Task<IActionResult> Deactivate(string id)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			user.IsActive = false;
			var result = await userManager.UpdateAsync(user);

			if (!result.Succeeded) return BadRequest(result.Errors);

			return RedirectToAction("Index");
		}

		// POST: Change user role
		[HttpPost]
		public async Task<IActionResult> ChangeRole(string id, string newRole)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			var currentRoles = await userManager.GetRolesAsync(user);
			await userManager.RemoveFromRolesAsync(user, currentRoles);
			var result = await userManager.AddToRoleAsync(user, newRole);

			if (!result.Succeeded) return BadRequest(result.Errors);

			return RedirectToAction("Index");
		}


		[HttpPost]
		public async Task<IActionResult> Activate(string id)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			user.IsActive = true;
			var result = await userManager.UpdateAsync(user);

			if (!result.Succeeded) return BadRequest(result.Errors);

			return RedirectToAction("Index");
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
	}
}