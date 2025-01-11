using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using Project.Repositories;

namespace Project.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IGenericRepository<Category> _repository;
	private readonly ApplicationDbContext _context;

	public HomeController(ILogger<HomeController> logger, IGenericRepository<Category> repository, ApplicationDbContext context)
	{
		_repository = repository;
		_logger = logger;
		_context = context;
	}

	public IActionResult Index()
	{

		ViewBag.Guitar = _context.Products
			.Where(p => p.ProductTypeId == 2)
			.OrderBy(p => Guid.NewGuid())
			.FirstOrDefault(); 
			
		ViewBag.Piano = _context.Products
			.Where(p => p.ProductTypeId == 8)
			.OrderBy(p => Guid.NewGuid())
			.FirstOrDefault();
			
		ViewBag.Drums = _context.Products
			.Where(p => p.ProductTypeId == 5)
			.OrderBy(p => Guid.NewGuid())
			.FirstOrDefault();




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
