using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Project.Helpers;
using Project.Models;
using Project.Repositories;
using Project.Specifications;

namespace Project.Controllers
{
	public class AdminController : Controller
	{
		private readonly ILogger<AdminController> _logger;
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IGenericRepository<SpecificationDefinition> _specDefRepo;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public AdminController(ILogger<AdminController> logger,
		IGenericRepository<Product> productRepo, IGenericRepository<SpecificationDefinition> specDefRepo, IWebHostEnvironment webHostEnvironment)
		{
			_logger = logger;
			_productRepo = productRepo;
			_specDefRepo = specDefRepo;
			_webHostEnvironment = webHostEnvironment;
		}



		public async Task<IActionResult> Index([FromQuery] ProductSpecParams specParams)
		{
			var spec = new ProductSpecification(specParams);
			var countSpec = new ProductsWithFiltersForCountSpecification(specParams);

			var products = await _productRepo.ListAsync(spec);
			var totalItems = await _productRepo.CountAsync(countSpec);

			var pagination = new Pagination<Product>(
				specParams.PageIndex,
				specParams.PageSize,
				totalItems,
				products
			);

			ViewBag.CurrentSort = specParams.Sort ?? "Sort By";

			var brandSpec = new BrandListSpecification();
			var brands = await _productRepo.ListAsync<Brand>(brandSpec);
			var brandList = new SelectList(brands, "Id", "Name", specParams.BrandId);

			var typeSpec = new TypeListSpecification();
			var types = await _productRepo.ListAsync<ProductType>(typeSpec);
			var typeList = new SelectList(types, "Id", "Name", specParams.TypeId);

			var categorySpec = new CategoryListSpecification();
			var categories = await _productRepo.ListAsync(categorySpec);
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

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var brandSpec = new BrandListSpecification();
			var brands = await _productRepo.ListAsync<Brand>(brandSpec);

			var typeSpec = new TypeListSpecification();
			var types = await _productRepo.ListAsync<ProductType>(typeSpec);



			var viewModel = new ProductCreateViewModel
			{
				Brands = new SelectList(brands, "Id", "Name"),
				ProductTypes = new SelectList(types, "Id", "Name"),

			};

			return View(viewModel);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductCreateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Brands = new SelectList(await _productRepo.ListAsync(new BrandListSpecification()), "Id", "Name");
				model.ProductTypes = new SelectList(await _productRepo.ListAsync(new TypeListSpecification()), "Id", "Name");


				var specDefSpec = new SpecificationDefinitionListSpecification(model.ProductTypeId);
				var specDefinition = await _specDefRepo.ListAsync(specDefSpec);

				model.SpecificationDetails = specDefinition.Select(sd => new SpecificationDetail
				{
					SpecificationDefinitionId = sd.Id,
					DisplayName = sd.DisplayName,
					DataType = sd.DataType,
					IsRequired = sd.IsRequired,
					Options = sd.Options,
					Value = model.SpecificationDetails.FirstOrDefault(s => s.SpecificationDefinitionId == sd.Id)?.Value
				}).ToList();

				return View(model);
			}

			string uniqueFileName = null;
			if (model.Photo != null && model.Photo.Length > 0)
			{
				uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Photo.FileName);
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "../wwwroot/assets/products");


				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
				var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
				if (!allowedExtensions.Contains(extension))
				{
					ModelState.AddModelError("Photo", "Niedozwolony format pliku. Dozwolone formaty to: .jpg, .jpeg, .png, .gif.");
					// Ponowne załadowanie danych i zwrócenie widoku
					// ...
				}

				string filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await model.Photo.CopyToAsync(fileStream);
				}
			}

			var product = new Product
			{
				Name = model.Name,
				Description = model.Description,
				PhotoUrl = uniqueFileName != null ? $"../assets/Products/{uniqueFileName}" : null,
				Price = model.Price,
				QuantityInStoct = model.QuantityInStock,
				BrandId = model.BrandId,
				ProductTypeId = model.ProductTypeId,
				ProductDetails = model.SpecificationDetails
			   .Where(sd => !string.IsNullOrEmpty(sd.Value))
			   .Select(sd => new ProductDetails
			   {
				   SpecificationDefinitionId = sd.SpecificationDefinitionId,
				   Value = sd.Value
			   }).ToList()
			};

			_productRepo.Add(product);
			await _productRepo.SaveAllAsync();
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> GetSpecifications(int productTypeId)
		{
			var specDefSpec = new SpecificationDefinitionListSpecification(productTypeId);
			var specDefinitions = await _specDefRepo.ListAsync(specDefSpec);

			var specs = specDefinitions.Select(sd => new SpecificationDetail
			{
				SpecificationDefinitionId = sd.Id,
				DisplayName = sd.DisplayName,
				DataType = sd.DataType,
				IsRequired = sd.IsRequired,
				Options = sd.Options
			}).ToList();

			return Json(specs);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
	}
}