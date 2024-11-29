using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Project.Helpers;
using Project.Models;
using Project.Repositories;
using Project.Specifications;

namespace Project.Controllers
{

	public class ProductController : Controller
	{
		private readonly ILogger<ProductController> _logger;

		private readonly IGenericRepository<Product> _repository;

		public ProductController(ILogger<ProductController> logger, IGenericRepository<Product> repository)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task<IActionResult> Index([FromQuery] ProductSpecParams specParams)
		{
			var spec = new ProductSpecification(specParams);
			var countSpec = new ProductsWithFiltersForCountSpecification(specParams);

			var products = await _repository.ListAsync(spec);
			var totalItems = await _repository.CountAsync(countSpec);

			var pagination = new Pagination<Product>(
				specParams.PageIndex,
				specParams.PageSize,
				totalItems,
				products
			);

			ViewBag.CurrentSort = specParams.Sort ?? "Sort By";

			var brandSpec = new BrandListSpecification();
			var brands = await _repository.ListAsync<Brand>(brandSpec);
			var brandList = new SelectList(brands, "Id", "Name", specParams.BrandId);

			var typeSpec = new TypeListSpecification();
			var types = await _repository.ListAsync<ProductType>(typeSpec);
			var typeList = new SelectList(types, "Id", "Name", specParams.TypeId);

			var categorySpec = new CategoryListSpecification();
			var categories = await _repository.ListAsync(categorySpec);
			var categoryList = new SelectList(categories, "Id", "Name", specParams.CategoryId);

			var viewModel = new ProductListViewModel
			{
				Pagination = pagination,
				Brands = brandList,
				Types = typeList,
				Categories = categoryList,
				SpecParams = specParams
			};

			return View(viewModel);
		}

		public async Task<IActionResult> Details(int id)
		{
			var spec = new ProductWithDetailSpecification(id);
			var product = await _repository.GetEntityWithSpec(spec);

			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		
		
		
		
		


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
		
	}
}