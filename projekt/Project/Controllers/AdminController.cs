using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
	
	[Authorize(Roles = ("Admin,Pracownik"))]
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


		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			// Pobieramy produkt z repozytorium
			var product = await _productRepo.GetByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			// Pobieramy listę marek i typów produktów do SelectList
			var brandSpec = new BrandListSpecification();
			var brands = await _productRepo.ListAsync<Brand>(brandSpec);

			var typeSpec = new TypeListSpecification();
			var types = await _productRepo.ListAsync<ProductType>(typeSpec);

			// Pobieramy definicje specyfikacji dla danego typu produktu
			var specDefSpec = new SpecificationDefinitionListSpecification(product.ProductTypeId);
			var specDefinition = await _specDefRepo.ListAsync(specDefSpec);

			// Budujemy listę SpecificationDetails w oparciu o definicje i już zapisane wartości
			var specificationDetails = specDefinition.Select(sd => new SpecificationDetail
			{
				SpecificationDefinitionId = sd.Id,
				DisplayName = sd.DisplayName,
				DataType = sd.DataType,
				IsRequired = sd.IsRequired,
				Options = sd.Options,
				Value = product.ProductDetails?
								.FirstOrDefault(pd => pd.SpecificationDefinitionId == sd.Id)
								?.Value // Podstawiamy istniejącą wartość, jeśli jest
			}).ToList();

			var viewModel = new ProductEditViewModel
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				QuantityInStock = product.QuantityInStoct,
				BrandId = product.BrandId,
				ProductTypeId = product.ProductTypeId,


				Brands = new SelectList(brands, "Id", "Name", product.BrandId),
				ProductTypes = new SelectList(types, "Id", "Name", product.ProductTypeId),
				SpecificationDetails = specificationDetails
			};

			return View(viewModel);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ProductEditViewModel model)
		{
			if (!ModelState.IsValid)
			{
				// Jeśli walidacja się nie powiedzie, ładujemy listy i specyfikacje ponownie
				model.Brands = new SelectList(await _productRepo.ListAsync(new BrandListSpecification()), "Id", "Name", model.BrandId);
				model.ProductTypes = new SelectList(await _productRepo.ListAsync(new TypeListSpecification()), "Id", "Name", model.ProductTypeId);

				var specDefSpec = new SpecificationDefinitionListSpecification(model.ProductTypeId);
				var specDefinition = await _specDefRepo.ListAsync(specDefSpec);

				model.SpecificationDetails = specDefinition.Select(sd => new SpecificationDetail
				{
					SpecificationDefinitionId = sd.Id,
					DisplayName = sd.DisplayName,
					DataType = sd.DataType,
					IsRequired = sd.IsRequired,
					Options = sd.Options,
					// Zachowujemy to, co użytkownik wprowadził
					Value = model.SpecificationDetails
								?.FirstOrDefault(s => s.SpecificationDefinitionId == sd.Id)
								?.Value
				}).ToList();

				return View(model);
			}

			var product = await _productRepo.GetByIdAsync(model.Id);
			if (product == null)
			{
				return NotFound();
			}

			// Nadpisujemy wartości
			product.Name = model.Name;
			product.Description = model.Description;
			product.Price = model.Price;
			product.QuantityInStoct = model.QuantityInStock;
			product.BrandId = model.BrandId;
			product.ProductTypeId = model.ProductTypeId;

			if (model.Photo != null && model.Photo.Length > 0)
			{
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
				var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();

				if (!allowedExtensions.Contains(extension))
				{
					ModelState.AddModelError("Photo", "Niedozwolony format pliku. Dozwolone: .jpg, .jpeg, .png, .gif.");
					// Ponownie wczytujemy dane do modelu
					model.Brands = new SelectList(await _productRepo.ListAsync(new BrandListSpecification()), "Id", "Name", model.BrandId);
					model.ProductTypes = new SelectList(await _productRepo.ListAsync(new TypeListSpecification()), "Id", "Name", model.ProductTypeId);
					return View(model);
				}

				string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Photo.FileName);
				string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "../wwwroot/assets/products");

				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await model.Photo.CopyToAsync(fileStream);
				}

				product.PhotoUrl = $"../assets/Products/{uniqueFileName}";
			}


			product.ProductDetails ??= new List<ProductDetails>();
			product.ProductDetails.Clear();
			product.ProductDetails = model.SpecificationDetails
				.Where(sd => !string.IsNullOrEmpty(sd.Value))
				.Select(sd => new ProductDetails
				{
					SpecificationDefinitionId = sd.SpecificationDefinitionId,
					Value = sd.Value
				})
				.ToList();

			_productRepo.Update(product);
			await _productRepo.SaveAllAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> GetDeleteModal(int id)
		{
			var product = await _productRepo.GetByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			return PartialView("_DeleteModal", product); // Widok partial dla modala
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var product = await _productRepo.GetByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			// Usuwamy produkt
			_productRepo.Remove(product);
			await _productRepo.SaveAllAsync();

			return RedirectToAction(nameof(Index)); // Powrót do listy produktów
		}



		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
	}
}