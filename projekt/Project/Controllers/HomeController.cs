using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;

namespace Project.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IGenericRepository<Category> _repository;

	public HomeController(ILogger<HomeController> logger, IGenericRepository<Category> repository)
	{
		_repository = repository;
		_logger = logger;
	}

	public IActionResult Index()
	{
		return View();
	}
	
	public async Task<IActionResult> Categories()
	{
		var categories = await _repository.ListAllAsync();
		return View(categories);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
